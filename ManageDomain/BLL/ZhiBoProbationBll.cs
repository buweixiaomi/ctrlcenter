using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class ZhiBoProbationBll
    {
        DAL.ZhiBoProbationDal dal = new DAL.ZhiBoProbationDal();
        public Models.ZhiBoProbation Add(Models.ZhiBoProbation model)
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.Add(dbconn, model);
            }
        }
        public Models.PageModel<Models.ZhiBoProbation> GetProbation(string keywords, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = dal.GetProbation(dbconn, keywords, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.ZhiBoProbation>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }
        public int AddRemark(int id, string remark)
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.AddRemark(dbconn, id, remark);
            }
        }
        public Models.ZhiBoProbation GetDetail(int id)
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.GetDetail(dbconn, id);
            }

        }
    }
}
