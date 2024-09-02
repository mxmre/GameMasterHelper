using GameMasterHelper.Logic.DnD;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;


namespace GameMasterHelper.Manage
{
    public abstract class ProgramDataBase
    {
        static private OpenFileDialog p_ofd;
        static private SaveFileDialog p_sfd;
        static private string p_creauresFilesFormat = "Creatures DB(*.json)|*.json";
        static public void InitDataBase()
        {
            p_ofd = new OpenFileDialog();
            p_sfd = new SaveFileDialog();
            P_CREATURES = new List<DnDCreature>();
        }
        static private bool LoadDataFromFile<TValue>(string filesFilter, string title, out TValue loadedData)
        {
            p_ofd.Filter = filesFilter;
            p_ofd.Title = title;
            bool? success = p_ofd.ShowDialog();
            if (success ?? false)
            {
                using (var file = new FileStream(p_ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    //var bf = new BinaryFormatter();
                    //loadedData = (TValue)bf.Deserialize(file);
                    loadedData = JsonSerializer.Deserialize<TValue>(file);
                }
            }
            else loadedData = default(TValue);
            return success ?? false;
        }
        static private bool SaveDataToFile<TValue>(string filesFilter, string title, TValue data)
        {
            p_sfd.Filter = filesFilter;
            p_sfd.Title = title;
            bool? success = p_sfd.ShowDialog();
            if (success ?? false)
            {
                using (var file = new FileStream(p_sfd.FileName, FileMode.Create,
                    FileAccess.Write))
                {
                    //var bf = new BinaryFormatter();
                    //bf.Serialize(file, data);
                    var jsonStr = JsonSerializer.Serialize(data);
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
                    file.Write(bytes, 0, bytes.Length);
                }
            }
            return success ?? false;
        }
        static public bool LoadCreaturesFromFile(out List<DnDCreature> creatures)
        {
            return LoadDataFromFile(p_creauresFilesFormat,
                "Открыть", out creatures);
        }
        static public bool SaveCreaturesToFile(List<DnDCreature> creatures)
        {
            return SaveDataToFile(p_creauresFilesFormat,
                "Сохранить", creatures);
        }
        static private List<DnDCreature> P_CREATURES;
        static public List<DnDCreature> Creatures { get => P_CREATURES; 
            set => P_CREATURES = value; }
    }
}
