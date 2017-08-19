using System.Diagnostics;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Базовый описатель типа устройства
    /// </summary>
    [DebuggerDisplay("{DeviceCommonType}: {Model}({Manufacturer})")]
    public class DeviceTypeDescriptor : IDeviceTypeDescriptor
    {
        public DeviceTypeDescriptor(string model, string deviceCommonType, string deviceManufacturer)
        {
            Manufacturer = deviceManufacturer;
            DeviceCommonType = deviceCommonType;
            Model = model;
        }

        /// <summary>
        /// для сериализатора
        /// </summary>
        public DeviceTypeDescriptor()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Ключ типа прибора
        /// </summary>
        public string TypeKey { get; set; }

        /// <summary>
        /// Модель прибора
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Класс устройств
        /// </summary>
        public string DeviceCommonType { get; set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public string Manufacturer { get; set; }

        public FunctionType Function { get; set; }

        public enum FunctionType
        {
            Controller,
            Converter,
        }

        public override string ToString()
        {
            return $"[{DeviceCommonType}]:{Model}";
        }

        public override bool Equals(object obj)
        {
            var tObj = obj as DeviceTypeDescriptor;
            if (tObj != null)
                return GetHashCode() == tObj.GetHashCode();

            return base.Equals(obj);
        }


        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                if (TypeKey != null) hash = (hash * 16777619) ^ TypeKey.GetHashCode();
                if (Model != null) hash = (hash * 16777619) ^ Model.GetHashCode();
                if (DeviceCommonType != null) hash = (hash * 16777619) ^ DeviceCommonType.GetHashCode();
                hash = (hash * 16777619) ^ Function.GetHashCode();
                return hash;
            }
        }
    }
}
