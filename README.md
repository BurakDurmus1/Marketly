# 🧩 .NET Core tabanlı mikroservis mimarisiyle geliştirilmiş bir e-ticaret altyapısı

## 🚀 Teknolojiler
- **.NET Core** – Ana uygulama çatısı  
- **Entity Framework Core** – ORM ve veri erişimi  
- **RabbitMQ** – Servisler arası mesajlaşma  
- **Redis** – Cache yönetimi  
- **SQL Server** – Veritabanı  
- **MSTest** – Unit test altyapısı  
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
