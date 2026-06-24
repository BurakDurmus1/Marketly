# 🧩 .NET Core tabanlı mikroservis mimarisiyle geliştirilmiş bir e-ticaret altyapısı

## 🚀 Teknolojiler
- **.NET Core 5/6/7/8** – Ana uygulama çatısı  
- **Entity Framework Core** – ORM ve veri erişimi  
- **RabbitMQ** – Servisler arası mesajlaşma  
- **Redis** – Cache yönetimi  
- **PostgreSQL / SQL Server** – Veritabanı  
- **xUnit / MSTest** – Unit test altyapısı  
- **Docker** – Servislerin containerize edilmesi  
- **Swagger** – API dokümantasyonu  

## 🧱 Mimari
Proje mikroservis yapısında tasarlanmıştır:

- **Catalog Service** – Ürün yönetimi  
- **Basket Service** – Sepet işlemleri  
- **Order Service** – Sipariş yönetimi  
- **Identity Service** – Kimlik doğrulama  
- **API Gateway** – Servis yönlendirme  

Her servis bağımsız olarak çalışır ve **RabbitMQ** üzerinden haberleşir.
