# ![](https://habrastorage.org/webt/cm/lf/kk/cmlfkkudc0hu_y_fou71l_ts8dq.png) RadRip (radio ripper)
Проект позволяет отслеживать треки на Shoutcast/Icecast радиостанциях.
.NET 5 WebAPI + клиент на WinUI 3.

## Сборки
* **MrErsh.RadioRipper.Client.Api** - запросы к **WebApi**;
* **MrErsh.RadioRipper.Client.App_v2** - десктоп клиент;
* **MrErsh.RadioRipper.Client.Mvvm** - MVVM компоненты для клиента;
* **MrErsh.RadioRipper.Client.Shared** - абстракции, используемые в **Client.Api** и **App_v2**;
* **MrErsh.RadioRipper.Client.UI** - обобщенные UI компоненты;
* **MrErsh.RadioRipper.Core** - компоненты для работы со стримами радиостанций;
* **MrErsh.RadioRipper.Dal** - dal;
* **MrErsh.RadioRipper.IdentityDal** - identity dal;
* **MrErsh.RadioRipper.Model** - модель;
* **MrErsh.RadioRipper.Shared** - хелперы для всех сборок;
* **MrErsh.RadioRipper.WebApi** - сам трекер и web api.
____
* /Fakes/**Api.Fake** - моки для **Client.Api**. Используется в конфигурации *Debug_FakeData*.
____
* /Demos/**UIDemo** - демо компонентов **Client.UI**
* /Demos/**RipperCoreDemo** - демо компонентов **Core**


## Примеры интерфейса
![](https://habrastorage.org/webt/qh/pm/r7/qhpmr79611cffvhrtacyxvs6mm4.png)
![](https://habrastorage.org/webt/bw/xl/ag/bwxlagdazjspc2pfttgavmrvdda.png)
