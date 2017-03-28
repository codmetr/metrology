using System;

namespace MarkerService.Filler
{
    public interface IFillerFactory<T>
    {
        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <typeparam name="Ttarget">��� ������� �� �������� ����������� ������</typeparam>
        /// <param name="Key">���� �������</param>
        /// <param name="result">������ �� �������� ����������� ������</param>
        /// <returns></returns>
        T FillMarker<Ttarget>(object Key, Ttarget result);
        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="ttarget">��� ������� �� �������� ����������� ������</param>
        /// <param name="Key">���� �������</param>
        /// <param name="item">������ �� �������� ����������� ������</param>
        /// <returns></returns>
        T FillMarker(Type ttarget, object Key, object item);
    }
}