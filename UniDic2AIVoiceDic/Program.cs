using Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniDic2AIVoiceDic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var csv = File.ReadAllText(@"E:\unidic-csj-3.1.1\lex_3_1.csv");
            var options = new CsvOptions()
            {
                HeaderMode = HeaderMode.HeaderAbsent,
            };
            var hash = new HashSet<string>();

            using (var sw = new StreamWriter(@"result.txt", false, Encoding.UTF8))
            using (var sw2 = new StreamWriter(@"kanji.txt", false, Encoding.Default))
            {
                var lines = CsvReader.ReadFromText(csv, options);
                foreach (var line in lines)
                {
                    var go = line[0];
                    var type = string.Join("-", line[4], line[5], line[6]);
                    var yomi = line[13];
                    var src = line[10];
                    if (type.Contains("助動詞") || type.Contains("数詞") || type.Contains("助詞") || type.Contains("連体詞") || type.Contains("非自立可能") || type.Contains("記号") || type.Contains("感動詞") || type.Contains("空白") || type.Contains("副詞") || type.Contains("一般") || type.Contains("人名") || type.Contains("形状詞") || type.Contains("接頭辞") || type.Contains("代名詞"))
                    {
                        continue;
                    }
                    if (Regex.IsMatch(go, "[=/㌁-㍖～゛~〜]") || Regex.IsMatch(go, "^[a-zA-Z0-9Ａ-Ｚａ-ｚ]*$"))
                    {
                        continue;
                    }
                    if (Regex.IsMatch(go, @"^\p{IsCJKUnifiedIdeographs}+$"))
                    {
                        if (StringUtil.ConvertEncoding(go, Encoding.GetEncoding("Shift_JIS")))
                        {
                            sw2.WriteLine($"{go} {yomi}");
                            continue;
                        }
                    }

                    if (type == "名詞-普通名詞-サ変可能")
                    {
                        type = "名詞-サ変接続";
                    }

                    if (type == "接尾辞-名詞的-サ変可能")
                    {
                        type = "名詞-サ変接続";
                    }
                    else if (type == "名詞-固有名詞-地名")
                    {
                        type = "名詞-固有名詞-地域-一般";
                    }
                    else if (type == "名詞-普通名詞-助数詞可能")
                    {
                        type = "名詞-一般";
                    }
                    else if (type == "接続詞-*-*")
                    {
                        type = "名詞-一般";
                    }
                    else if (type == "接尾辞-動詞的-*")
                    {
                        type = "名詞-一般";
                    }
                    else if (type == "接尾辞-形容詞的-*")
                    {
                        type = "名詞-一般";
                    }
                    if (!hash.Contains(go.Han2Zen()))
                    {
                        sw.WriteLine(string.Join(";", type, go, "2000", yomi, $"0-{Math.Min(src.Length, yomi.Length)}:*"));
                        hash.Add(go);
                    }
                }
            }
        }
    }
}
