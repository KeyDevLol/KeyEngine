using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    public static class SpriteLoader
    {
        public static string[] FindExtensions = new string[] { ".png", ".jpg" };

        /// <summary>
        /// Возвращает лист со всеми спрайтами из указанной папки
        /// </summary>
        /// <param name="dirPath">Путь к папке</param>
        /// <returns></returns>
        public static List<Texture> LoadToList(string dirPath)
        {
            List<Texture> sprites = new List<Texture>();

            var files = Directory.GetFiles(dirPath).Where(file => FindExtensions.Any(file.ToLower().EndsWith)).ToList();

            foreach (var file in files)
            {
                byte[] bytes = File.ReadAllBytes(file);

                sprites.Add(new Texture(bytes));
            }

            return sprites;
        }        
        /// <summary>
        /// Возвращает массив со всеми спрайтами из указанной папки
        /// </summary>
        /// <param name="dirPath">Путь к папке</param>
        /// <returns></returns>
        public static Texture[] LoadToArray(string dirPath)
        {
            List<Texture> sprites = new List<Texture>();

            var files = Directory.GetFiles(dirPath).Where(file => FindExtensions.Any(file.ToLower().EndsWith)).ToList();

            foreach (var file in files)
            {
                byte[] bytes = File.ReadAllBytes(file);

                sprites.Add(new Texture(bytes));
            }

            return sprites.ToArray();
        }        
        /// <summary>
        /// Возвращает словарь со всеми спрайтами из указанной папки, ключ это название файла спрайта
        /// </summary>
        /// <param name="dirPath">Путь к папке</param>
        /// <returns></returns>
        public static Dictionary<string, Texture> LoadToDictionary(string dirPath)
        {
            Dictionary<string, Texture> sprites = new Dictionary<string, Texture>();

            var files = Directory.GetFiles(dirPath).Where(file => FindExtensions.Any(file.ToLower().EndsWith)).ToList();

            foreach (var file in files)
            {
                byte[] bytes = File.ReadAllBytes(file);

                sprites.Add(Path.GetFileNameWithoutExtension(file), new Texture(bytes));
            }

            return sprites;
        }
    }
}
