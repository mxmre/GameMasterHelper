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


namespace GameMasterHelper.Manage
{
    public abstract class Module
    {
        static private OpenFileDialog p_ofd;
        static private SaveFileDialog p_sfd;
        static private string p_ModuleFilesFormat = "Module(*.gmhmod)|*.gmhmod";
        static public string TempFolder
        {
            get
            {
                return "~TEMP\\";
            }
        }
        static public string TempModuleFolder
        {
            get
            {
                return Path.Combine(TempFolder, "module\\");
            }
        }
        static public string AppDataFolder
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return path == string.Empty ? "\\GameMasterHelper\\" : Path.Combine(path + "\\GameMasterHelper\\");
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
                var jsonStr = JsonSerializer.Serialize(objects, typeof(List<TValue>));
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
                objects = JsonSerializer.Deserialize<List<TValue>>(file);
            }
            
        }
        static public bool SaveModule()
        {
            string tempDirectioryName = Path.Combine( AppDataFolder 
                 + TempModuleFolder);
            if (Directory.Exists(tempDirectioryName))
                Directory.Delete(tempDirectioryName, true);
            Directory.CreateDirectory(tempDirectioryName);

            SaveObjectsToJson(P_CREATURES, tempDirectioryName + "creatures.json");


            Directory.CreateDirectory(tempDirectioryName + "images\\");
            Directory.CreateDirectory(tempDirectioryName + "images\\creatures\\");
            SaveImages(P_CREATURE_IMAGES, tempDirectioryName + "images\\creatures\\");
            bool success = false;
            if (success = p_sfd.ShowDialog() ?? false)
            {
                if (File.Exists(p_sfd.FileName))
                    File.Delete(p_sfd.FileName);
                ZipFile.CreateFromDirectory(tempDirectioryName, p_sfd.FileName);
                
            }
            if (Directory.Exists(tempDirectioryName))
                Directory.Delete(tempDirectioryName, true);
            return success;
        }

        
        static public bool LoadModule()
        {

            string tempDirectioryName = AppDataFolder
                 + TempModuleFolder;
            if (Directory.Exists(tempDirectioryName))
                Directory.Delete(tempDirectioryName, true);
            bool success = false;
            if (success = p_ofd.ShowDialog() ?? false)
            {
                ZipFile.ExtractToDirectory(p_ofd.FileName, tempDirectioryName);

                LoadObjectsFromJson(out P_CREATURES, tempDirectioryName + "creatures.json");
                LoadImages(out P_CREATURE_IMAGES, tempDirectioryName + "images\\creatures\\");
            }
            if (Directory.Exists(tempDirectioryName))
                Directory.Delete(tempDirectioryName, true);
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
