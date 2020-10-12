namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using XmlRepository.Base;
    using XmlRepository.Base.Data;
    using XmlRepository.Dto;

    public class ConsoleHostedService : HostedService
    {
        private readonly IDataSource<Book> _bookSource;
        private readonly IDataSource<Book> _source;

        public ConsoleHostedService(IDataSource<Book> bookSource, IDataSource<Book> source)
        {
            _bookSource = bookSource;
            _source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Book> books = null;

            //while (!cancellationToken.IsCancellationRequested)
            //{
            // Create the delegate
            //var readBooks = new Func<IEnumerable<Book>>(_bookSource.Read);
            //var readBooks = new Func<IEnumerable<Book>>(_source.Read);
            var readBooks = new Func<SourceEntity<Book>>(_source.Read);

            //var action = new Func<IEnumerable<Book>>(_bookSource.Read);

            // Initiate the async call
            var result = await Task.Run(() => readBooks.Invoke());
            //books = result.ToList();
            books = result.Entities.ToList();

            //}
        }
    }
}
