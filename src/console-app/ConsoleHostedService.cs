namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using XmlRepository.Base.Data;
    using XmlRepository.Dto;

    public class ConsoleHostedService : HostedService
    {
        private readonly IDataSource<Book> _bookSource;

        public ConsoleHostedService(IDataSource<Book> bookSource)
        {
            _bookSource = bookSource;
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
                var readBooks = new Func<IEnumerable<Book>>(_bookSource.Read);

                //var action = new Func<IEnumerable<Book>>(_bookSource.Read);

                // Initiate the async call
                var result = await Task.Run(() => readBooks.Invoke());
                books = result.ToList();
                
            //}
        }
    }
}
