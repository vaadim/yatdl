# yatdl
Yet Another TODO List
1) Создать на уровне БД логин admin (пароль : admin)
2) Выполнить скрипт .\Data\InitialCreate.sql, предварительно заменив имя инстанса сервера на имеющееся значение, например:

'C:\Program Files\Microsoft SQL Server\MSSQL12.PATRICK\MSSQL\DATA\ToDo.mdf' заменить на 
'C:\Program Files\Microsoft SQL Server\MSSQL11.DEV1\MSSQL\DATA\ToDo.mdf'

3) Изменить значение DataSource в connectionString в файле .\Web\Config\connectionStrings.config на имеющееся значение БД, например:

Data Source=.\PATRICK; заменить на 
Data Source=FRGSDEV1\DEV1;

4) В inetmgr добавить новый сайт, например yatdl, по физическому пути сайта разместить содержимое папки builds (опубликованный в файловую систему сайт)

5) Дать права modify пользователю iis apppool\yatdl для папки App_Data

6) Зайти на сайт, при создании БД задаются 2 пользователя: admin (пароль admin) и user (user)
