using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Linq;
using Anotar.Catel;
using Catel.MVVM;
using Catel.Runtime.Serialization;
using Drachenkatze.PresetMagician.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PresetMagician.Core.Data;

namespace PresetMagician.Core.Models.NativeInstrumentsResources
{
    [Model]
    public class NativeInstrumentsResource : ModelBase
    {
        private static HashSet<string> _editableProperties = new HashSet<string>();
        public const string DIST_DATABASE_DIRECTORY = "dist_database";
        public const string IMAGES_DIRECTORY = "image";

        public override HashSet<string> GetEditableProperties()
        {
            return _editableProperties;
        }

        public Color Color { get; set; } = new Color();

        [IncludeInSerialization] public Categories Categories { get; set; } = new Categories();
        [IncludeInSerialization] public ShortNames ShortNames { get; set; } = new ShortNames();

        public ResourceState ColorState { get; } = new ResourceState();
        public ResourceState ShortNamesState { get; } = new ResourceState();
        public ResourceState CategoriesState { get; } = new ResourceState();

        #region Images

        public ResourceImage VB_logo { get; private set; } = new ResourceImage(279, 47, "VB_logo.png");
        public ResourceImage VB_artwork { get; private set; } = new ResourceImage(96, 47, "VB_artwork.png");
        public ResourceImage MST_artwork { get; private set; } = new ResourceImage(134, 66, "MST_artwork.png");
        public ResourceImage MST_plugin { get; private set; } = new ResourceImage(127, 70, "MST_plugin.png");
        public ResourceImage MST_logo { get; private set; } = new ResourceImage(240, 196, "MST_logo.png");
        public ResourceImage OSO_logo { get; private set; } = new ResourceImage(417, 65, "OSO_logo.png");

        public List<ResourceImage> ResourceImages { get; } = new List<ResourceImage>();

        #endregion

        public static string GetNativeInstrumentsResourcesDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                "NI Resources");
        }


        private static string GetPluginFilenameOutputDirectory(Plugin plugin)
        {
            var re = new Regex("([^a-z0-9\\s\\-_])");
            var fileName = Path.GetFileNameWithoutExtension(plugin.PluginLocation.DllPath);
            var lcFileName = fileName.ToLower();

            var mangledFileName = re.Replace(lcFileName, "");

            if (mangledFileName != lcFileName)
            {
                return mangledFileName + "_" + HashUtils.getFormattedMD5Hash(fileName);
            }

            return lcFileName;
        }

        public static string GetDistDatabaseDirectory(Plugin plugin, bool dllFilename = false)
        {
            if (!dllFilename)
            {
                return Path.Combine(GetNativeInstrumentsResourcesDirectory(), DIST_DATABASE_DIRECTORY,
                    plugin.PluginVendor.ToLower(), plugin.GetEffectivePluginName().ToLower());
            }

            return Path.Combine(GetNativeInstrumentsResourcesDirectory(), DIST_DATABASE_DIRECTORY,
                GetPluginFilenameOutputDirectory(plugin));
        }

        public static string GetImageDirectory(Plugin plugin, bool dllFilename = false)
        {
            if (!dllFilename)
            {
                return Path.Combine(GetNativeInstrumentsResourcesDirectory(), IMAGES_DIRECTORY,
                    plugin.PluginVendor.ToLower(),
                    plugin.GetEffectivePluginName().ToLower());
            }

            return Path.Combine(GetNativeInstrumentsResourcesDirectory(), IMAGES_DIRECTORY,
                GetPluginFilenameOutputDirectory(plugin));
        }

        public NativeInstrumentsResource()
        {
            ResourceImages.Add(VB_logo);
            ResourceImages.Add(VB_artwork);
            ResourceImages.Add(MST_logo);
            ResourceImages.Add(MST_plugin);
            ResourceImages.Add(MST_artwork);
            ResourceImages.Add(OSO_logo);
        }

        private static string GetHexColor(System.Windows.Media.Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public string GetCategoriesJson()
        {
            if (Categories.CategoryDB.Count == 0)
            {
                Categories.CategoryDB.Add(new CategoryDB());
            }

            Categories.CategoryDB.First().Categories.Clear();

            foreach (var category in Categories.CategoryNames)
            {
                Categories.CategoryDB.First().Categories.Add(category.Name);
            }

            return JsonConvert.SerializeObject(Categories);
        }

        public void Save(Plugin plugin)
        {
            LogTo.Debug($"Begin saving metadata for {plugin.GetEffectivePluginName()}");
            var files = GetFiles(plugin);
            var resourcesDirectory = GetDistDatabaseDirectory(plugin);
            var imagesDirectory = GetImageDirectory(plugin);
            var dllFilenameResourcesDirectory = GetDistDatabaseDirectory(plugin, true);
            var dllFilenameImagesDirectory = GetImageDirectory(plugin, true);

            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            if (!Directory.Exists(dllFilenameImagesDirectory))
            {
                Directory.CreateDirectory(dllFilenameImagesDirectory);
            }

            foreach (var image in ResourceImages)
            {
                image.Save(imagesDirectory);
                image.Save(dllFilenameImagesDirectory);
            }

            if (!Directory.Exists(resourcesDirectory))
            {
                Directory.CreateDirectory(resourcesDirectory);
            }

            if (!Directory.Exists(dllFilenameResourcesDirectory))
            {
                Directory.CreateDirectory(dllFilenameResourcesDirectory);
            }


            if (ColorState.ShouldSave)
            {
                Color.VB_bgcolor = GetHexColor(Color.BackgroundColor);
                File.WriteAllText(files["color"], JsonConvert.SerializeObject(Color));
                File.WriteAllText(files["color2"], JsonConvert.SerializeObject(Color));
                ColorState.State = ResourceStates.FromDisk;
            }

            if (ShortNamesState.ShouldSave)
            {
                File.WriteAllText(files["shortname"], JsonConvert.SerializeObject(ShortNames));
                File.WriteAllText(files["shortname2"], JsonConvert.SerializeObject(ShortNames));
                ShortNamesState.State = ResourceStates.FromDisk;
            }

            try
            {
                File.WriteAllText(files["categories"], GetCategoriesJson());
                File.WriteAllText(files["categories2"], GetCategoriesJson());
                CategoriesState.State = ResourceStates.FromDisk;
            }
            catch (Exception e)
            {
                LogTo.Error($"Error occured while saving categories: {e.Message}");
                LogTo.Debug(e.StackTrace);
            }

            try
            {
                var metaFile = Path.Combine(resourcesDirectory, plugin.GetEffectivePluginName().ToLower() + ".meta");
                var dllFilenameMetaFile = Path.Combine(dllFilenameResourcesDirectory,
                    GetPluginFilenameOutputDirectory(plugin) + ".meta");
                if (!File.Exists(metaFile))
                {
                    CreateMetaFile(plugin, "dist_database", metaFile);
                }

                if (!File.Exists(dllFilenameMetaFile))
                {
                    CreateMetaFile(plugin, "dist_database", dllFilenameMetaFile);
                }

                var dllFilenameImageMetaFile = Path.Combine(dllFilenameImagesDirectory,
                    GetPluginFilenameOutputDirectory(plugin) + ".meta");
                var imageMetaFile = Path.Combine(imagesDirectory, plugin.GetEffectivePluginName().ToLower() + ".meta");
                if (!File.Exists(imageMetaFile))
                {
                    CreateMetaFile(plugin, "image", imageMetaFile);
                }

                if (!File.Exists(dllFilenameImageMetaFile))
                {
                    CreateMetaFile(plugin, "image", dllFilenameImageMetaFile);
                }
            }
            catch (Exception e)
            {
                LogTo.Error($"Error occured while saving metadata: {e.Message}");
                LogTo.Debug(e.StackTrace);
            }
        }

        private static void CreateMetaFile(Plugin plugin, string dbType, string outputFile)
        {
            var doc = new XDocument {Declaration = new XDeclaration("1.0", "UTF-8", "no")};

            var resource = new XElement("resource");
            resource.SetAttributeValue("version", "1.0");

            var vendor = new XElement("vendor") {Value = plugin.PluginVendor};
            resource.Add(vendor);

            var name = new XElement("name") {Value = plugin.GetEffectivePluginName()};
            resource.Add(name);

            var type = new XElement("type") {Value = dbType};
            resource.Add(type);

            doc.Add(resource);

            doc.Save(outputFile);
        }

        private static Dictionary<string, string> GetFiles(Plugin plugin)
        {
            var files = new Dictionary<string, string>();
            var resourcesDirectory = GetDistDatabaseDirectory(plugin);
            var dllFilenameResourcesDirectory = GetDistDatabaseDirectory(plugin, true);

            files.Add("color", Path.Combine(resourcesDirectory, "color.json"));
            files.Add("shortname", Path.Combine(resourcesDirectory, "shortname.json"));
            files.Add("categories", Path.Combine(resourcesDirectory, "categories.json"));

            files.Add("color2", Path.Combine(dllFilenameResourcesDirectory, "color.json"));
            files.Add("shortname2", Path.Combine(dllFilenameResourcesDirectory, "shortname.json"));
            files.Add("categories2", Path.Combine(dllFilenameResourcesDirectory, "categories.json"));

            return files;
        }

        public void LoadFromJObject(JObject obj)
        {
            Color.BackgroundColor =
                (System.Windows.Media.Color) ColorConverter.ConvertFromString("#" + obj["bgColor"]);
            ColorState.State = ResourceStates.FromWeb;

            ShortNames.VB_shortname = (string) obj["shortName_VB"];
            ShortNames.MKII_shortname = (string) obj["shortName_MKII"];
            ShortNames.MST_shortname = (string) obj["shortName_MST"];
            ShortNames.MIKRO_shortname = (string) obj["shortName_MIKRO"];
            ShortNamesState.State = ResourceStates.FromWeb;

            VB_logo.ReplaceFromBase64((string) obj["image_VB_logo"]);
            VB_artwork.ReplaceFromBase64((string) obj["image_VB_artwork"]);
            MST_logo.ReplaceFromBase64((string) obj["image_MST_logo"]);
            MST_artwork.ReplaceFromBase64((string) obj["image_MST_artwork"]);
            MST_plugin.ReplaceFromBase64((string) obj["image_MST_plugin"]);
            OSO_logo.ReplaceFromBase64((string) obj["image_OSO_logo"]);


            var categoryStrings = ((string) obj["categories"]).Split(',').ToList();

            Categories.CategoryNames.Clear();

            foreach (var categoryString in categoryStrings)
            {
                Categories.CategoryNames.Add(new Category {Name = categoryString});
            }

            CategoriesState.State = ResourceStates.FromWeb;
        }

        public void Load(Plugin plugin)
        {
            if (plugin == null || !plugin.HasMetadata)
            {
                return;
            }

            var files = GetFiles(plugin);


            if (File.Exists(files["color"]))
            {
                Color = JsonConvert.DeserializeObject<Color>(
                    File.ReadAllText(files["color"]));

                try
                {
                    Color.BackgroundColor =
                        (System.Windows.Media.Color) ColorConverter.ConvertFromString("#" + Color.VB_bgcolor);
                }
                catch (Exception e)
                {
                    var colorFile = files["color"];
                    LogTo.Error(
                        $"Unable to load color information from {colorFile} because the color {Color.VB_bgcolor} is probably invalid. {e.GetType().FullName}: {e.Message}");
                }

                ColorState.State = ResourceStates.FromDisk;
            }
            else
            {
                Color.BackgroundColor = (System.Windows.Media.Color) ColorConverter.ConvertFromString("#FFFFFF");
            }


            if (File.Exists(files["shortname"]))
            {
                ShortNames = JsonConvert.DeserializeObject<ShortNames>(
                    File.ReadAllText(files["shortname"]));

                ShortNamesState.State = ResourceStates.FromDisk;
            }

            if (File.Exists(files["categories"]))
            {
                Categories.CategoryNames.Clear();

                Categories = JsonConvert.DeserializeObject<Categories>(
                    File.ReadAllText(files["categories"]));

                if (Categories.CategoryDB.Count == 1)
                {
                    var categoryStrings = Categories.CategoryDB.First().Categories.ToArray();
                    foreach (var categoryString in categoryStrings)
                    {
                        Categories.CategoryNames.Add(new Category {Name = categoryString});
                    }
                }
                else
                {
                    var categoryDb = new CategoryDB
                    {
                        FileType = plugin.PluginType == Plugin.PluginTypes.Instrument ? "INST" : "FX"
                    };

                    Categories.CategoryDB.Add(categoryDb);
                    Categories.Vendor = plugin.PluginVendor;
                    Categories.Product = plugin.GetEffectivePluginName();
                }

                CategoriesState.State = ResourceStates.FromDisk;
            }


            var imagesDirectory = GetImageDirectory(plugin);

            foreach (var resourceImage in ResourceImages)
            {
                resourceImage.Load(imagesDirectory);
            }
        }

        public bool HasChanges(Plugin plugin)
        {
            var originalResources = new NativeInstrumentsResource();
            originalResources.Load(plugin);

            if (originalResources.Categories.CategoryNames.Count != Categories.CategoryNames.Count)
            {
                return true;
            }

            foreach (var categoryName in originalResources.Categories.CategoryNames)
            {
                if (!(from cat in Categories.CategoryNames where cat.Name == categoryName.Name select cat).Any())
                {
                    return true;
                }
            }


            foreach (var image in ResourceImages)
            {
                if (image.State.ShouldSave)
                {
                    return true;
                }
            }

            return
                GetHexColor(originalResources.Color.BackgroundColor) != GetHexColor(Color.BackgroundColor) ||
                originalResources.ShortNames.VB_shortname != ShortNames.VB_shortname ||
                originalResources.ShortNames.MST_shortname != ShortNames.MST_shortname ||
                originalResources.ShortNames.MKII_shortname != ShortNames.MKII_shortname ||
                originalResources.ShortNames.MIKRO_shortname != ShortNames.MIKRO_shortname;
        }

        public enum ResourceStates
        {
            Empty,
            FromDisk,
            FromWeb,
            AutomaticallyGenerated,
            UserModified
        }
    }
}