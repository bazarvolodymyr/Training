using Microsoft.Extensions.Options;
using Training.Application.Interfaces;

namespace Training.API.Media
{
    public class LocalMediaFileStore : IMediaFileStore
    {
        private readonly string _root;

        public LocalMediaFileStore(IWebHostEnvironment environment, IOptions<MediaOptions> options)
        {
            var configured = options.Value.RootPath;
            _root = Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(environment.ContentRootPath, configured);
            Directory.CreateDirectory(_root);
        }

        public async Task SaveAsync(string storedName, Stream content, CancellationToken cancellationToken)
        {
            await using var file = new FileStream(
                PathOf(storedName),
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);
            await content.CopyToAsync(file, cancellationToken);
        }

        public Stream? OpenRead(string storedName)
        {
            var path = PathOf(storedName);
            if (!File.Exists(path))
            {
                return null;
            }

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void Delete(string storedName)
        {
            var path = PathOf(storedName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private string PathOf(string storedName)
        {
            if (storedName != Path.GetFileName(storedName) || storedName.Contains("..", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Media file name is not safe.");
            }

            return Path.Combine(_root, storedName);
        }
    }
}
