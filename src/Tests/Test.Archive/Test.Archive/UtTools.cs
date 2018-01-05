using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ArchiveData;
using ArchiveData.DTO;
using Moq;
using PressureSensorData;
using SQLiteArchive;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Test.Archive
{
    static class UtTools
    {
        /// <summary>
        /// Сформировать интерфейс доступа к тестовой базе данных
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static IDataPool GetDataPool(TestContext context)
        {
            var testDb = "test.db";
            Debug.WriteLine($"TestResultsDirectory: {context.TestResultsDirectory}");
            var listDevDescriptors = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor("devModel", "devCommonType", "manufacturer")
            };
            var dictMoq = new Mock<IDictionaryPool>();
            dictMoq.Setup(foo => foo.DeviceTypes).Returns(listDevDescriptors);
            var resTypes = new Dictionary<string, Type>() { { "devType", typeof(PressureSensorResult) } };
            var confTypes = new Dictionary<string, Type>() { { "devType", typeof(PressureSensorConfig) } };
            var datapool = DataPool.Load(dictMoq.Object, resTypes, confTypes, Path.Combine(context.TestResultsDirectory, testDb), (msg) => Debug.WriteLine(msg));
            return datapool;
        }

        /// <summary>
        /// Сформировать тестовые данные
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <param name="config"></param>
        internal static void FillTestData(out TestResultID id, out PressureSensorResult result, out PressureSensorConfig config)
        {
            id = new TestResultID()
            {
                TargetDeviceKey = "keyType",
                DeviceType = "devType",
            };
            result = new PressureSensorResult()
            {
                Assay = "Assay1",
                CommonResult = "42",
                VisualCheckResult = "Ok",
                Leak = "NoLeak",
                Points = new List<PressureSensorPointResult>()
                {
                    GetResPoint(1, 2),
                    GetResPoint(2, 4),
                }
            };
            config = PressureSensorConfig.GetDefault();
            config.User = "User1";
            config.ReportNumber = "10";
            config.CertificateNumber = "11";
            config.NumberLastCheck = "321";
            config.EthalonPressure = new EthalonDescriptor()
            {
                Title = "Ethalon1",
                SensorType = "Pressure",
                SerialNumber = "12",
                RegNum = "22",
                Category = "0.02",
                ErrorClass = "1",
                CheckCertificateNumber = "12",
                CheckCertificateDate = "NewYear"
            };
            config.EthalonVoltage = new EthalonDescriptor()
            {
                Title = "Ethalon2",
                SensorType = "Volage",
                SerialNumber = "121",
                RegNum = "221",
                Category = "0.021",
                ErrorClass = "11",
                CheckCertificateNumber = "121",
                CheckCertificateDate = "NewYear1"
            };
            config.Points = new List<PressureSensorPoint>()
            {
                GetPoint(1, 2),
                GetPoint(2, 4),
            };
            config.VpiMax = 1;
            config.VpiMin = 0;
            config.Unit = "mmHg";
            config.TolerancePercentVpi = 0.01;
            config.ToleranceDelta = 0.02;
            config.TolerancePercentSigma = 0.03;
            config.OutputRange = OutGange.I0_5mA;
        }

        /// <summary>
        /// Сводмировать результат по точке
        /// </summary>
        /// <param name="press"></param>
        /// <param name="volt"></param>
        /// <returns></returns>
        internal static PressureSensorPointResult GetResPoint(double press, double volt)
        {
            return new PressureSensorPointResult()
            {
                PressurePoint = press,
                PressureUnit = "mmHg",
                PressureValue = press * 1.1,
                VoltagePoint = volt,
                VoltageUnit = "mA",
                VoltageValue = volt * 0.95,
                VoltageValueBack = volt * 1.1
            };
        }

        /// <summary>
        /// Сформировать точку
        /// </summary>
        /// <param name="press"></param>
        /// <param name="volt"></param>
        /// <returns></returns>
        internal static PressureSensorPoint GetPoint(double press, double volt)
        {
            return new PressureSensorPoint()
            {
                PressurePoint = press,
                PressureUnit = "mmHg",
                VoltagePoint = volt,
                VoltageUnit = "mA",
                Tollerance = 0.1,
            };
        }

        /// <summary>
        /// Сравнение данных двух ID
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static bool CompareData(TestResultID x, TestResultID y)
        {
            if (x.Id != y.Id)
                return false;
            if (x.CreateTime != y.CreateTime)
                return false;
            if (x.Timestamp != y.Timestamp)
                return false;
            if (x.TargetDeviceKey != y.TargetDeviceKey)
                return false;
            if (x.DeviceType != y.DeviceType)
                return false;
            if (x.SerialNumber != y.SerialNumber)
                return false;
            if (x.CommonResult != y.CommonResult)
                return false;
            if (x.Note != y.Note)
                return false;
            return true;
        }

        /// <summary>
        /// Сравнение данных двух результатов
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static bool CompareData(PressureSensorResult x, PressureSensorResult y)
        {
            if (x.Assay != y.Assay)
                return false;
            if (x.CommonResult != y.CommonResult)
                return false;
            if (x.Leak != y.Leak)
                return false;
            if (x.VisualCheckResult != y.VisualCheckResult)
                return false;
            if (x.Points.Except(y.Points, new PressureSensorPointResultComparrer()).Count() != 0)
                return false;
            return true;
        }


        /// <summary>
        /// Сравнение данных двух конфигураций
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static bool CompareData(PressureSensorConfig x, PressureSensorConfig y)
        {
            if (x.User != y.User)
                return false;
            if (x.ReportNumber != y.ReportNumber)
                return false;
            if (x.ReportDate != y.ReportDate)
                return false;
            if (x.CertificateNumber != y.CertificateNumber)
                return false;
            if (x.CertificateDate != y.CertificateDate)
                return false;
            if (x.Master != y.Master)
                return false;
            if (x.Name != y.Name)
                return false;
            if (x.SensorType != y.SensorType)
                return false;
            if (x.SensorModel != y.SensorModel)
                return false;
            if (x.RegNum != y.RegNum)
                return false;
            if (x.NumberLastCheck != y.NumberLastCheck)
                return false;
            if (x.SerialNumber != y.SerialNumber)
                return false;
            if (x.CheckedParameters != y.CheckedParameters)
                return false;
            if (x.ChecklLawBase != y.ChecklLawBase)
                return false;
            if (x.Company != y.Company)
                return false;
            if (Math.Abs(x.Temperature - y.Temperature) > double.Epsilon)
                return false;
            if (Math.Abs(x.Humidity - y.Humidity) > double.Epsilon)
                return false;
            if (Math.Abs(x.DayPressure - y.DayPressure) > double.Epsilon)
                return false;
            if (Math.Abs(x.CommonVoltage - y.CommonVoltage) > double.Epsilon)
                return false;
            if (!CompareData(x.EthalonPressure, y.EthalonPressure))
                return false;
            if (!CompareData(x.EthalonVoltage, y.EthalonVoltage))
                return false;
            if (Math.Abs(x.VpiMax - y.VpiMax) > double.Epsilon)
                return false;
            if (Math.Abs(x.VpiMin - y.VpiMin) > double.Epsilon)
                return false;
            if (x.Unit != y.Unit)
                return false;
            if (Math.Abs(x.TolerancePercentVpi - y.TolerancePercentVpi) > double.Epsilon)
                return false;
            if (Math.Abs(x.ToleranceDelta - y.ToleranceDelta) > double.Epsilon)
                return false;
            if (Math.Abs(x.TolerancePercentSigma - y.TolerancePercentSigma) > double.Epsilon)
                return false;
            if (x.OutputRange != y.OutputRange)
                return false;

            if (x.Points.Except(y.Points, new PressureSensorPointComparrer()).Count() != 0)
                return false;
            return true;
        }

        /// <summary>
        /// Сравнение данных двух описателей эталона
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static bool CompareData(EthalonDescriptor x, EthalonDescriptor y)
        {
            if (x.Title != y.Title)
                return false;
            if (x.SensorType != y.SensorType)
                return false;
            if (x.SerialNumber != y.SerialNumber)
                return false;
            if (x.RegNum != y.RegNum)
                return false;
            if (x.Category != y.Category)
                return false;
            if (x.ErrorClass != y.ErrorClass)
                return false;
            if (x.CheckCertificateNumber != y.CheckCertificateNumber)
                return false;
            if (x.CheckCertificateDate != y.CheckCertificateDate)
                return false;
            return true;
        }


        internal class PressureSensorPointComparrer : IEqualityComparer<PressureSensorPoint>
        {
            public bool Equals(PressureSensorPoint x, PressureSensorPoint y)
            {
                if (x == null || y == null)
                    return false;
                if (Math.Abs(x.PressurePoint - y.PressurePoint) > double.Epsilon)
                    return false;
                if (x.PressureUnit != y.PressureUnit)
                    return false;
                if (Math.Abs(x.VoltagePoint - y.VoltagePoint) > double.Epsilon)
                    return false;
                if (x.VoltageUnit != y.VoltageUnit)
                    return false;
                if (Math.Abs(x.Tollerance - y.Tollerance) > double.Epsilon)
                    return false;
                return true;
            }

            public int GetHashCode(PressureSensorPoint obj)
            {
                return base.GetHashCode();
            }
        }

        internal class PressureSensorPointResultComparrer : IEqualityComparer<PressureSensorPointResult>
        {
            public bool Equals(PressureSensorPointResult x, PressureSensorPointResult y)
            {
                if (x == null || y == null)
                    return false;
                if (Math.Abs(x.PressurePoint - y.PressurePoint) > double.Epsilon)
                    return false;
                if (x.PressureUnit != y.PressureUnit)
                    return false;
                if (Math.Abs(x.VoltagePoint - y.VoltagePoint) > double.Epsilon)
                    return false;
                if (x.VoltageUnit != y.VoltageUnit)
                    return false;
                if (Math.Abs(x.PressureValue - y.PressureValue) > double.Epsilon)
                    return false;
                if (Math.Abs(x.VoltageValue - y.VoltageValue) > double.Epsilon)
                    return false;
                if (Math.Abs(x.VoltageValueBack - y.VoltageValueBack) > double.Epsilon)
                    return false;
                return true;
            }

            public int GetHashCode(PressureSensorPointResult obj)
            {
                return base.GetHashCode();
            }
        }

    }
}
