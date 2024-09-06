using GameMasterHelper.Logic.DnD;
using GameMasterHelper.Manage.Collections;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.IO.Packaging;
using System.Text.Json.Serialization;


namespace GameMasterHelper.Manage
{
    public class JsonDnDCreatureConverter  : JsonConverter<DnDCreature>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(DnDCreature).IsAssignableFrom(type);
        }

        public override DnDCreature Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                    || reader.TokenType != JsonTokenType.PropertyName
                    || reader.GetString() != "DnDCreatureType")
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            DnDCreature baseClass;
            DnDCreatureBuilder.DnDCreatureType typeDiscriminator = 
                (DnDCreatureBuilder.DnDCreatureType)reader.GetInt32();
            switch (typeDiscriminator)
            {
                case DnDCreatureBuilder.DnDCreatureType.Default:
                    if (!reader.Read() || reader.GetString() != "TypeValue")
                    {
                        throw new JsonException();
                    }
                    if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonException();
                    }
                    baseClass = (DnDCreature)JsonSerializer.Deserialize(ref reader, typeof(DnDCreature));
                    break;
                case DnDCreatureBuilder.DnDCreatureType.MagicCaster:
                    if (!reader.Read() || reader.GetString() != "TypeValue")
                    {
                        throw new JsonException();
                    }
                    if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonException();
                    }
                    baseClass = (DnDCreatureMagicCaster)JsonSerializer.Deserialize(ref reader, typeof(DnDCreatureMagicCaster));
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return baseClass;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DnDCreature value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is DnDCreatureMagicCaster caster)
            {
                writer.WriteNumber("DnDCreatureType", (int)DnDCreatureBuilder.DnDCreatureType.MagicCaster);
                writer.WritePropertyName("TypeValue");
                JsonSerializer.Serialize(writer, caster);
            }
            else if (value is DnDCreature creature)
            {
                writer.WriteNumber("DnDCreatureType", (int)DnDCreatureBuilder.DnDCreatureType.Default);
                writer.WritePropertyName("TypeValue");
                JsonSerializer.Serialize(writer, creature);
            }
            else
            {
                throw new NotSupportedException();
            }

            writer.WriteEndObject();
        }
    }

    public abstract class DataManager
    {
        static private OpenFileDialog p_ofd;
        static private SaveFileDialog p_sfd;
        static private string p_ModuleFilesFormat = "DataManager(*.gmhmod)|*.gmhmod";
        public abstract class Paths
        {
            static public string TempFolder
            {
                get
                {
                    return Path.Combine(AppDataFolder, "~TEMP\\");
                }
            }
            static public string TempModuleFolder
            {
                get
                {
                    return Path.Combine(TempFolder, "module\\");
                }
            }
            static public string ImageFolder
            {
                get
                {
                    return Path.Combine(TempModuleFolder, "images\\");
                }
            }
            static public string CreatureImageFolder
            {
                get
                {
                    return Path.Combine(ImageFolder, "creatures\\");
                }
            }
            static public string CreatureSaveFile
            {
                get
                {
                    return Path.Combine(TempModuleFolder, "creatures.json");
                }
            }
            static public string AppDataFolder
            {
                get
                {
                    string progName = "\\GameMasterHelper\\";
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    return path == string.Empty ? progName : Path.Combine(path + progName);
                }
            }
        }
        
        static public void Init()
        {
            p_ofd = new OpenFileDialog();
            p_sfd = new SaveFileDialog();
            p_sfd.Filter = p_ModuleFilesFormat;

            p_ofd.Filter = p_ModuleFilesFormat;
            p_currentModule = new Module();
        }
        static private void SaveImages(Catalog<BitmapSource> images, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return;
            var imagesDict = images.Items;

            foreach (var image in imagesDict)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image.Value));

                using (var fileStream = new System.IO.FileStream(directoryPath + image.Key.ToString() + ".png", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);

                }
            }
        }
        static public BitmapSource BitmapSourceFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        static private void LoadImages(out Catalog<BitmapSource> images, string directoryPath)
        {

            if (!Directory.Exists(directoryPath))
            {
                images = null;
                return;
            }
            images = Module.DefaultCreatureImages;
            images.BeginInsertInit();
            var imagesPaths = Directory.GetFiles(directoryPath);

            foreach (var path in imagesPaths)
            {
                ulong realImageKey = 0;
                bool isCorrectImageKey = ulong.TryParse(
                    Path.GetFileNameWithoutExtension(Path.GetFileName(path)), out realImageKey);
                if (!isCorrectImageKey)
                {
                    throw new Exception("Wrong Image Name!");
                }
                images.Insert(realImageKey, BitmapSourceFromUri(new Uri(path, UriKind.Absolute)));
            }
            images.EndInsertInit();
        }
        static public JsonSerializerOptions JsonOptions 
        { 
            get => new JsonSerializerOptions
                {
                    Converters = { new JsonDnDCreatureConverter() },
                    WriteIndented = true,
                }; 
        }
        static public void SaveObjectsToJson<TValue>(List<TValue> objects, string filePath)
        {
            if (!File.Exists(filePath))
                return;
            using (var file = new FileStream(filePath, FileMode.Create,
                    FileAccess.Write))
            {
                var jsonStr = JsonSerializer.Serialize(objects, typeof(List<TValue>), JsonOptions);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
                file.Write(bytes, 0, bytes.Length);
            }
        }
        static public void LoadObjectsFromJson<TValue>(out List<TValue> objects, string filePath)
        {
            objects = null;
            if (!File.Exists(filePath))
            {
                return;
            }
                
            using (var file = new FileStream(filePath, FileMode.Open,
                    FileAccess.Read))
            {
                objects = JsonSerializer.Deserialize<List<TValue>>(file, JsonOptions);
            }
            
        }
        static private void DeleteTempSaveDirectories()
        {
            if (Directory.Exists(Paths.TempFolder))
                Directory.Delete(Paths.TempFolder, true);
        }
        static protected void CreateTempSaveDirectories()
        {
            DeleteTempSaveDirectories();
            Directory.CreateDirectory(Paths.TempModuleFolder);

            Directory.CreateDirectory(Paths.ImageFolder);
            Directory.CreateDirectory(Paths.CreatureImageFolder);
        }
        static private void SaveCreatures(Module module)
        {
            if(!(module.Creatures == null || module.Creatures.Count == 0))
            {
                SaveObjectsToJson(module.Creatures, Paths.CreatureSaveFile);
                SaveImages(module.CreatureImages, Paths.CreatureImageFolder);
            }
        }
        static private bool ReadyToSave(Module module)
        {
            bool result = !(module.Creatures == null || module.Creatures.Count == 0);
            return result;
        }
        static public bool SaveModuleData(Module module)
        {
            if (module == null)
                throw new NullReferenceException(nameof(module));
            bool success = false;
            if(ReadyToSave(module))
            {
                CreateTempSaveDirectories();

                SaveCreatures(module);
            
                if (success = p_sfd.ShowDialog() ?? false)
                {
                    if (File.Exists(p_sfd.FileName))
                        File.Delete(p_sfd.FileName);
                    ZipFile.CreateFromDirectory(Paths.TempModuleFolder, p_sfd.FileName);
                }
                DeleteTempSaveDirectories();
            }
            return success;
        }

        static private void LoadCreatures(Module module)
        {
            List<DnDCreature> lc = null;
            Catalog<BitmapSource> cbs = null;
            LoadObjectsFromJson(out lc, Paths.CreatureSaveFile);
            LoadImages(out cbs, Paths.CreatureImageFolder);
            module.Creatures = lc;
            module.CreatureImages = cbs;
        }
        static public bool LoadModuleData(Module module)
        {
            if(module == null)
                throw new NullReferenceException(nameof(module));
            DeleteTempSaveDirectories();
            Directory.CreateDirectory(Paths.TempModuleFolder);
            bool success = false;
            if (success = p_ofd.ShowDialog() ?? false)
            {
                ZipFile.ExtractToDirectory(p_ofd.FileName, Paths.TempModuleFolder);
                LoadCreatures(module);
            }
            DeleteTempSaveDirectories();
            return success;
        }
        static public bool LoadModuleData(out Module module)
        {
            module = new Module();
            bool result = LoadModuleData(module);
            if (!result) module = null;
            return result;
        }
        static public bool SaveModuleData()
        {
            return SaveModuleData(p_currentModule);
        }
        static public bool LoadModuleData()
        {
            return LoadModuleData(p_currentModule);
        }

        static private Module p_currentModule;

        static public Module CurrentModule
        {
            get { return p_currentModule; }
            set { p_currentModule = value; }
        }

    }
}
