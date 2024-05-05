using Neo4j.Driver;
using Projem.Models;


namespace Projem.Services
{
    public class Neo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService()
        {
            _driver = GraphDatabase.Driver("neo4j+s://0fbefadd.databases.neo4j.io", AuthTokens.Basic("neo4j", "3jQtcUlFPkMyNSsLvu8p4E7rj7EnGKN988QZHN5s2KA"));
        }
        public async Task<List<string>> GetAllNodes()
        {
            var nodes = new List<string>();

            using (var session = _driver.AsyncSession())
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync("MATCH (n) RETURN n LIMIT 20");
                    return await cursor.ToListAsync();
                });

                foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    foreach (var prop in node.Properties)
                    {
                        var propValue = prop.Value.As<string>();
                        if (!string.IsNullOrEmpty(propValue))
                        {
                            nodes.Add(propValue);
                        }
                    }
                }
            }

            return nodes;
        }

        public async Task<List<KullaniciSifre>> GetAllUsers()
        {
            var users = new List<KullaniciSifre>();

            using (var session = _driver.AsyncSession())
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync("MATCH (k:Kullanicilar)-[a:Ait]->(g:GirisBilgileri) RETURN k.adi AS Ad, k.soyadi AS Soyad, g.Sifre AS Sifre, a.KayitTarihi AS KayitTarihi");

                    return await cursor.ToListAsync();
                });

                foreach (var record in result)
                {
                    var user = new KullaniciSifre
                    {
                        Ad = record["Ad"].As<string>(),
                        Soyad = record["Soyad"].As<string>(),
                        Sifre = record["Sifre"].As<string>(),
                        KayitTarihi = record["KayitTarihi"].As<DateTime>()

                    };
                    users.Add(user);
                }
            }

            return users;
        }

       public async Task AddNewUser(KullaniciSifre user)
{
    try
    {
        using (var session = _driver.AsyncSession())
        {
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync("CREATE (k:Kullanicilar {adi: $adi, soyadi: $soyad})",
                                  new { adi = user.Ad, soyad = user.Soyad });
                await tx.RunAsync("CREATE (g:GirisBilgileri {Sifre: $sifre})",
                                  new { sifre = user.Sifre });
                await tx.RunAsync("MATCH (k:Kullanicilar {adi: $adi}), (g:GirisBilgileri {Sifre: $sifre}) " +
                                  "CREATE (k)-[:Ait{KayitTarihi:$Kayittarihim}]->(g)",
                                  new { adi = user.Ad, sifre = user.Sifre , Kayittarihim=user.KayitTarihi});
            });
        }
    }
    catch (Exception ex)
    {
        // Hata durumunda burada işleme alınabilir.
        Console.WriteLine("Hata oluştu: " + ex.Message);
        throw;
    }
}

    }
}
