using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.Repo
{
    //todo: прототип, необходимо доделывать
    /// <summary>
    /// Универсальный мапер
    /// </summary>
    /// <remarks>
    /// 1) Базовая функциональность: Преобразование объекта произвольного типа в коллекцию текстовый ключь - значение с возможностью восстановления в исходный тип.
    /// 2) Ключь должен быть уникален в пределах типа конкретного объекта
    /// 3) Значение должно храниться в типе поддерживаемом системой преобразования
    /// 4) Типы хранящие структуру результата должны иметь публичные поля с обязательными публичными get/set. Так же у них должен быть конструктор по умолчанию.
    /// 5) Все object должны разрешаться в реальный тип.
    /// 
    /// Планы:
    /// 6) Потенциально возможно увеличить производительность разбора формированем коллекции Expression для заданных типов и при повторном обнаружении просто вызывать их.
    /// </remarks>
    public class UnyRepo
    {
        public Dictionary<IEnumerable<Node>, Data> ToDict<T>(T item)
        {
            throw new NotImplementedException();
        }

        public T FromDict<T>(Dictionary<IEnumerable<Node>, Data> arch)
        {
            throw new NotImplementedException();
        }

        public Dictionary<IEnumerable<Node>, Data> UpdateDict<T>(Dictionary<IEnumerable<Node>, Data> arch, T item)
        {
            throw new NotImplementedException();
        }

        public class Data
        {
        }

        public class Node
        {
            /// <summary>
            /// 
            /// </summary>
            public Node Parrent { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<Node> Cilds { get; set; } 
        }
    }
}
