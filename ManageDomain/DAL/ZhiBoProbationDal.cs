using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class ZhiBoProbationDal
    {
        public Models.ZhiBoProbation Add(CCF.DB.DbConn dbconn, Models.ZhiBoProbation model)
        {
            string sql = @"INSERT INTO `zhiboprobation`
(
`name`,
`profession`,
`companyNum`,
`mobile`,
`QQ`,
`createTime`,
`Remark`)
VALUES
(
@name,
@profession,
@companyNum,
@mobile,
@QQ,
now(),
@Remark);";
            dbconn.ExecuteSql(sql, new
            {
                name = model.Name,
                profession = model.Profession,
                companyNum = model.CompanyNum,
                mobile = model.Mobile,
                QQ = model.QQ,
                Remark = model.Remark ?? ""
            });
            model.ID = dbconn.GetIdentity();
            return model;
        }
        public List<Models.ZhiBoProbation> GetProbation(CCF.DB.DbConn dbconn, string keywords, int pno, int pagesize, out int totalcount)
        {
            string whereconn = "";
            if (!string.IsNullOrEmpty(keywords))
            {
                whereconn += @" where  (  name like concat('%',@keywords,'%') or
                                profession like concat('%',@keywords,'%') or 
                                mobile  like concat('%',@keywords,'%') or 
                                QQ like concat('%',@keywords,'%') ) ";
            }
            string sql = string.Format("select * from zhiboprobation {0} order by createTime desc limit @startindex,@pagesize", whereconn);
            var models = dbconn.Query<Models.ZhiBoProbation>(sql, new { keywords = keywords, startindex = pagesize * (pno - 1), pagesize = pagesize }).ToList();
            string countsql = string.Format("select count(1) from zhiboprobation {0} ; ", whereconn);
            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords });
            return models;
        }
        public int AddRemark(CCF.DB.DbConn dbconn, int id, string remark)
        {
            string sql = " update zhiboprobation set remark=@remark where id=@id";
            int res = dbconn.ExecuteSql(sql, new { remark = remark, id = id });
            return res;
        }
        public Models.ZhiBoProbation GetDetail(CCF.DB.DbConn dbconn, int id)
        {
            string sql = " select * from zhiboprobation where id=@id ";
            return dbconn.Query<Models.ZhiBoProbation>(sql, new { id = id }).FirstOrDefault();
        }
    }
}
