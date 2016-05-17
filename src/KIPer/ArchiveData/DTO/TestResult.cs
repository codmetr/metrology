﻿using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;

namespace KipTM.Archive.DTO
{
    public class TestResult
    {
        /// <summary>
        /// Пользователь проводивший проверку
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Пометка к поверке
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Лаборатория
        /// </summary>
        public string Laboratory { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public string Humidity { get; set; }

        /// <summary>
        /// Тип поверки
        /// </summary>
        public string CheckType { get; set; }

        /// <summary>
        /// Канал устройства
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Ключь типа проверяемого устройства
        /// </summary>
        public string TargetDeviceKey { get; set; }

        /// <summary>
        /// Целевое устройство
        /// </summary>
        public DeviceDescriptor TargetDevice { get; set; }

        /// <summary>
        /// Набор использованных эталонов
        /// </summary>
        public List<DeviceDescriptor> Etalon { get; set; }

        /// <summary>
        /// Результаты
        /// </summary>
        public Dictionary<ParameterDescriptor, ParameterResult> Results { get; set; }
    }
}