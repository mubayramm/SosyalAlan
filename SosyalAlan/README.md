\# SosyalAlan



ASP.NET Core 8 Web API ile geliştirilmiş basit bir sosyal ağ uygulaması.



\## Özellikler



\- Kullanıcı kayıt ve giriş (JWT token)

\- Kullanıcı arama

\- Arkadaşlık isteği gönderme, kabul etme veya reddetme

\- Arkadaşlar arasında mesajlaşma



\## Teknolojiler



\- ASP.NET Core 8 Web API

\- Entity Framework Core 8

\- SQL Server LocalDB

\- JWT Authentication

\- BCrypt şifre hashleme

\- Swagger / OpenAPI



\## API Endpoint'leri



| Method | Endpoint | Açıklama |

|--------|----------|----------|

| POST | /api/auth/kayit | Kayıt ol |

| POST | /api/auth/giris | Giriş yap, token al |

| GET | /api/kullanicilar | Kullanıcıları listele ve ara |

| POST | /api/arkadaslik | Arkadaşlık isteği gönder |

| PATCH | /api/arkadaslik/{id} | İsteği kabul et veya reddet |

| GET | /api/arkadaslik | Arkadaş listesi |

| POST | /api/mesaj | Mesaj gönder |

| GET | /api/mesaj/{id} | Konuşmayı görüntüle |

