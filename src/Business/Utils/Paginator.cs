namespace Business.Utils
{
    using System.Collections.Generic;

    namespace Domain.Utils
    {


        public class Paginator<TEntity>
        {

            public virtual List<TEntity> Content { get; set; }

            public int CountItems { get; set; }

            public int ItemsPerPage { get; set; }

            public int CountPages { get; set; }

            public int CurrentPage { get; set; }

            public int NextPage { get; set; }

            public int PreviusPage { get; set; }

            public int FirstItemOfPage { get; set; }

            public int LastItemOfPage { get; set; }

            public Paginator() { }


            public Paginator(List<TEntity> content, int countItems, int currentPage, int itemsPerPage)
            {
                Content = content;
                CountItems = countItems;
                ItemsPerPage = itemsPerPage;

                CurrentPage = currentPage;
                CountPages = (CountItems / ItemsPerPage);
                if (CountItems % ItemsPerPage > 0)
                {
                    CountPages++;
                }
                NextPage = CountPages > CurrentPage ? CurrentPage + 1 : 0;
                PreviusPage = CurrentPage > 1 ? CurrentPage - 1 : 0;
                FirstItemOfPage = PreviusPage * ItemsPerPage + 1;
                LastItemOfPage = CurrentPage * itemsPerPage - (CurrentPage * itemsPerPage - CountItems);

                if (CountItems > CurrentPage * itemsPerPage)
                {
                    LastItemOfPage = CurrentPage * itemsPerPage;
                }
                else
                {
                    LastItemOfPage = CountItems;
                }

            }

        }
    }
}
