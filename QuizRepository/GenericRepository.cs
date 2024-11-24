using QuizAppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizAppModel.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuizAppRepository
{
    public class GenericRepository<T>  : IPaths where T : IIdentifiable
    {
        private string _path;
        protected List<T> Data;

        public GenericRepository(string path)
        {
            _path = path;
            Data = LoadData();
        }

        public void AddData(T data)
        {
            Data.Add(data);
            UpdateData();
        }

        public T GetData(Predicate<T> predicate)

        {
            return Data.FirstOrDefault(cur => predicate(cur));
        }

        public void ModifyData(int id, Action<T> modifyAction)
        {
            T existingData = GetData(cur => cur.Id == id);

            if (existingData != null)
            {
                modifyAction(existingData);
                UpdateData();
            }
            else
                Console.WriteLine($"Element with ID {id} was not found.");
        }


        public void DeleteData(int id)
        {
            T ElemToDelete = GetData(cur => id == cur.Id);
            if (ElemToDelete != null)
            {
                Data.Remove(ElemToDelete);
                UpdateData();
            }
            else
                Console.WriteLine("Element not found");
        }

        public void UpdateData()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            using (StreamWriter writer = new StreamWriter(_path))
            {
                string json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }


        public List<T> LoadData()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            using (StreamReader reader = new StreamReader(_path))
            {
                string json = reader.ReadToEnd();
                if (json == "") return new List<T>();
                return JsonSerializer.Deserialize<List<T>>(json);
            }
        }
    }
}