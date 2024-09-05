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

    public abstract class Module
    {
        static private OpenFileDialog p_ofd;
        static private SaveFileDialog p_sfd;
        static private string p_ModuleFilesFormat = "Module(*.gmhmod)|*.gmhmod";
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
        static public void InitModule()
        {
            p_ofd = new OpenFileDialog();
            p_sfd = new SaveFileDialog();
            p_sfd.Filter = p_ModuleFilesFormat;

            p_ofd.Filter = p_ModuleFilesFormat;
            P_CREATURES = new List<DnDCreature>();
            CreatureImagesCatalog = new Catalog<BitmapSource>(1);
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
            images = new Catalog<BitmapSource>(1);
            var imagesPaths = Directory.GetFiles(directoryPath);

            foreach (var path in imagesPaths)
            {
                ulong realImageKey = 0;
                bool isCorrectImageKey = ulong.TryParse(Path.GetFileNameWithoutExtension(Path.GetFileName(path)), out realImageKey);
                if (!isCorrectImageKey)
                {
                    throw new Exception("Wrong Image Name!");

                }

                if (images.AddItem(BitmapSourceFromUri(new Uri(path, UriKind.Absolute))) != realImageKey)
                {
                    throw new Exception("Real image key and addable image key is not equal!");
                }
            }
        }
        static public void SaveObjectsToJson<TValue>(List<TValue> objects, string directoryPath)
        {
            using (var file = new FileStream(directoryPath, FileMode.Create,
                    FileAccess.Write))
            {
                //var xml = new XmlSerializer(typeof(TValue[]));
                ////loadedData = (TValue)bf.Deserialize(file);
                //xml.Serialize(file, data.ToArray());
                ////var bf = new BinaryFormatter();
                ////bf.Serialize(file, data);
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonDnDCreatureConverter() },
                    WriteIndented = true,
                };
                var jsonStr = JsonSerializer.Serialize(objects, typeof(List<TValue>), options);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
                file.Write(bytes, 0, bytes.Length);
            }
        }
        static public void LoadObjectsFromJson<TValue>(out List<TValue> objects, string filePath)
        {
            objects = null;
            using (var file = new FileStream(filePath, FileMode.Open,
                    FileAccess.Read))
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonDnDCreatureConverter() },
                    WriteIndented = true,
                };
                objects = JsonSerializer.Deserialize<List<TValue>>(file, options);
            }
            
        }
        static private void DeleteTempSaveDirectories()
        {
            if (Directory.Exists(TempFolder))
                Directory.Delete(TempFolder, true);
        }
        static protected void CreateTempSaveDirectories()
        {
            DeleteTempSaveDirectories();
            Directory.CreateDirectory(TempModuleFolder);

            Directory.CreateDirectory(ImageFolder);
            Directory.CreateDirectory(CreatureImageFolder);
        }
        static public bool SaveModule()
        {
            CreateTempSaveDirectories();

            SaveObjectsToJson(P_CREATURES, CreatureSaveFile);
            SaveImages(P_CREATURE_IMAGES, CreatureImageFolder);
            bool success = false;
            if (success = p_sfd.ShowDialog() ?? false)
            {
                if (File.Exists(p_sfd.FileName))
                    File.Delete(p_sfd.FileName);
                ZipFile.CreateFromDirectory(TempModuleFolder, p_sfd.FileName);
            }
            DeleteTempSaveDirectories();
            return success;
        }

        
        static public bool LoadModule()
        {
            DeleteTempSaveDirectories();
            Directory.CreateDirectory(TempModuleFolder);
            bool success = false;
            if (success = p_ofd.ShowDialog() ?? false)
            {
                ZipFile.ExtractToDirectory(p_ofd.FileName, TempModuleFolder);

                LoadObjectsFromJson(out P_CREATURES, CreatureSaveFile);
                LoadImages(out P_CREATURE_IMAGES, CreatureImageFolder);
            }
            DeleteTempSaveDirectories();
            return success;
        }

        static private List<DnDCreature> P_CREATURES;
        static public List<DnDCreature> Creatures { get => P_CREATURES; 
            set => P_CREATURES = value; }

        static private Catalog<BitmapSource> P_CREATURE_IMAGES;
        static public Catalog<BitmapSource> CreatureImagesCatalog
        {
            get => P_CREATURE_IMAGES; set => P_CREATURE_IMAGES = value;
        }
    }
}
