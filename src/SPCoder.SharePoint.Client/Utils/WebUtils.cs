using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.Utils
{
    internal static class WebUtils
    {
        public static string MakeAbsoluteUrl(Web web, string serverRelativeUrl)
        {
            // e.g. 
            // Web: 
            // https://tenant.sharepoint.com/sites/foo/bar
            // serverRelative:
            // /sites/foo/bar/documents/sample.txt
            web.EnsureProperties(w => w.Url);

            Uri webUri = new Uri(web.Url);
            string tenantStub = $"{webUri.Scheme}://{webUri.Authority}";

            return $"{tenantStub}{serverRelativeUrl}";
        }

        public static List<string> GetAssociatedSiteUrlsForHub(ClientContext ctx, Guid hubSiteId)
        {
            List<string> urls = new List<string>();

            KeywordQuery keywordQuery = new KeywordQuery(ctx);
            keywordQuery.QueryText = $"contentclass=sts_site  DepartmentId:{{{hubSiteId}}}";
            keywordQuery.SelectProperties.Add("Path");
            keywordQuery.SourceId = new Guid("8413cd39-2156-4e00-b54d-11efd9abdb89"); // Local SharePoint Results
            keywordQuery.TrimDuplicates = false;
            
            SearchExecutor searchExecutor = new SearchExecutor(ctx);
            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
            ctx.ExecuteQuery();

            foreach (var resultTable in results.Value)
            {
                foreach (var resultRow in resultTable.ResultRows)
                {
                    var url = resultRow["Path"] as string;
                    urls.Add(url);
                }
            }

            return urls;
        }
    }
}
