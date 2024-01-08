# Currency Fetcher

Данный программный продукт, предназначен для получения и обзора динамики  изменения курсов иностранных валют. Данные о курсах и список доступных  иностранных валют получаются с помощью [АРІ сервиса национального банка  Беларуси](https://www.nbrb.by/apihelp/exrates).

## Функциональные возможности

- Получение списка доступных иностранных валют за указанный период и запись полученных данных в текстовый файл в формате JSON;
- Чтение данных из текстового файла и вывод на экран в динамический  список с возможностью редактирования поля курса или аббревиатуры  выбранной строки. Измененные данные автоматически записываются в  исходный файл;
- Отображение динамики изменения курсов иностранных валют на графике.

## Установка и запуск

1. Скачать [последнюю версию](https://github.com/GreyMarty/CurrencyFetcher/releases/latest);
2. Распаковать архив;
3. Запустить файл **`setup.exe`**.

## Дополнительные библиотеки

- [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged) - автоматический вызов события `PropertyChanged` в классах, реализующих `INotifyPropertyChanged`;
- [Microsoft.Xaml.Behaviors.Wpf](https://github.com/microsoft/XamlBehaviorsWpf) - обработка событий посредством команд;