using System.Collections.Generic;
using InApp;
using NUnit.Framework;
using UnityEngine;

public class UrlsCreatorUTest
{
    [Test]
    public void UrlsCreatorUTestSimplePasses()
    {
        UrlsCreator urls = new UrlsCreator(new Pathes());
        List<string> ls = urls.Create();

        List<string> correctLs = new List<string>()
        {
            "https://spb.cian.ru/cat.php?deal_type=sale&district%5B0%5D=723&engine_version=2&object_type%5B0%5D=1&offer_type=flat&totime=864000&room1=1&maxtarea=38",
            "https://spb.cian.ru/cat.php?deal_type=sale&district%5B0%5D=723&engine_version=2&object_type%5B0%5D=1&offer_type=flat&totime=864000&room1=1&mintarea=38&maxtarea=42",
            "https://spb.cian.ru/cat.php?deal_type=sale&district%5B0%5D=723&engine_version=2&object_type%5B0%5D=1&offer_type=flat&totime=864000&room1=1&mintarea=42",
        };

        List<string> noContainedUrls = new List<string>();

        Debug.Log(ls.Count);

        foreach (string item in correctLs)
        {
            if (ls.Contains(item) == false)
            {
                noContainedUrls.Add(item);
            }
        }

        if (noContainedUrls.Count > 0)
        {
            Assert.Fail($"Urls not contained ({noContainedUrls.Count}): \n{string.Join("\n", noContainedUrls)}");
        }
    }
}
