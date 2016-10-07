namespace ArchiveData.Repo
{
    public interface IRepo<T>
    {
        T Load(ITreeEntity node);
        ITreeEntity Save(T entity);
        ITreeEntity Update(ITreeEntity node, T entity);
    }
}