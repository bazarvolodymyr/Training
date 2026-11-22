namespace Training.Application.Interfaces
{
    public interface IMediaFileStore
    {
        Task SaveAsync(string storedName, Stream content, CancellationToken cancellationToken);

        Stream? OpenRead(string storedName);

        void Delete(string storedName);
    }
}
