using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Controllers
{
    public class Pager
    {
        public static int DEFAULT_PAGE_SIZE = 20;
        public static int DEFAULT_TARGET_PAGE_NO = 1;
        protected int targetPageNo;
        protected int pageSize;
        protected int countOfPages;
        protected int countOfRecords;
        protected int countOfRecordsInCurrentPage;
        private int indexOfFirstItemOfCurrentPage;
        private int prePageNo;
        private int nextPageNo;
        private bool existPrePage;
        private bool existNextPage;

        private Pager(int targetPageNo, int pageSize, int countOfRecords)
        {
            this.targetPageNo = 1;
            this.pageSize = DEFAULT_PAGE_SIZE;
            countOfPages = 0;
            this.countOfRecords = 0;
            countOfRecordsInCurrentPage = 0;
            indexOfFirstItemOfCurrentPage = 0;
            prePageNo = 0;
            nextPageNo = 0;
            existPrePage = false;
            existNextPage = false;
            this.targetPageNo = targetPageNo;
            this.pageSize = pageSize;
            this.countOfRecords = countOfRecords;
            Calculate();
        }

        public static Pager GetInstance(int targetPageNo, int pageSize, int countOfRecords)
        {
            return new Pager(targetPageNo, pageSize, countOfRecords);
        }

        public bool ExistNextPage { 
            get 
            {
                return existNextPage;
            }
        }

        public bool ExistPrePage
        {
            get 
            {
                return existPrePage;
            }
        }

        public int IndexOfFirstItemOfCurrentPage
        {
            get 
            {
                return indexOfFirstItemOfCurrentPage;
            }
        }

        public int NextPageNo
        {
            get 
            {
                return nextPageNo;
            }
        }

        public int PrePageNo
        {
            get 
            {
                return prePageNo;
            }
        }

        public int CountOfPages
        {
            get 
            {
                return countOfPages;
            }
        }

        public int CountOfRecords
        {
            get 
            {
                return countOfRecords;
            }
        }

        public int CountOfRecordsInCurrentPage
        {
            get 
            {
                return countOfRecordsInCurrentPage;
            }
            set
            {
                this.countOfRecordsInCurrentPage = value;
            }
        }

        public int PageSize
        {
            get 
            {
                return pageSize;
            }
        }

        public int TargetPageNo
        {
            get 
            {
                return targetPageNo;
            }
        }

        private void Calculate()
        {
            if (targetPageNo <= 0)
            {
                targetPageNo = 1;
            }
            if (countOfRecords < 0)
            {
                countOfRecords = 0;
            }
            if (pageSize <= 0)
            {
                pageSize = DEFAULT_PAGE_SIZE;
            }
            if (countOfRecords % pageSize == 0)
                countOfPages = countOfRecords / pageSize;
            else
                countOfPages = countOfRecords / pageSize + 1;
            if (countOfPages <= 0)
                targetPageNo = 0;
            else
                if (countOfPages < targetPageNo)
                    targetPageNo = countOfPages;
            if (targetPageNo <= 1)
            {
                prePageNo = 1;
                existPrePage = false;
            }
            else
            {
                prePageNo = targetPageNo - 1;
                existPrePage = true;
            }
            if (targetPageNo <= 1)
            {
                if (targetPageNo < 1)
                {
                    nextPageNo = 1;
                    existNextPage = false;
                }
                else
                    if (countOfPages >= 2)
                    {
                        nextPageNo = 2;
                        existNextPage = true;
                    }
                    else
                    {
                        nextPageNo = 1;
                        existNextPage = false;
                    }
            }
            else
                if (targetPageNo >= countOfPages)
                {
                    nextPageNo = countOfPages;
                    existNextPage = false;
                }
                else
                {
                    nextPageNo = targetPageNo + 1;
                    existNextPage = true;
                }
            if (targetPageNo <= 0)
                indexOfFirstItemOfCurrentPage = 0;
            else
                indexOfFirstItemOfCurrentPage = (targetPageNo - 1) * pageSize;
        }

    }
}