using System.IO;
using System.Windows;
using AssistantProcessor.Enums;
using AssistantProcessor.Models;
using Microsoft.VisualBasic.FileIO;

namespace AssistantProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CoreFile coreFile;
        public string tr =
            "1. У хворого 70-ти років раптово з'явилися слабкість, запаморочення. Виникло багаторазове\r\nвипорожнення прямої кишки калом з рідиною темно-вишневого кольору, згортками крові.\r\nЗанамнезу: за останні 5 місяців схуд на 17 кг, останні 3 місяці закрепи. Об'єктивно:\r\nблідий, у β-лівій здухвинній ділянці пальпується нерухомий болісний інфільтрат. Роздуті\r\nпоперечний та низхідний відділи ободової кишки. Які інструментальні дослідження на\r\nпершому етапі будуть найбільш доцільними?\r\nA. Ректороманоскопія, іригоскопія, фіброколоноскопія *\r\nB. Ультразвукове дослідження органів черевної порожнини, оглядова рентгенографія\r\nгрудної клітки\r\nC. Рентгенконтрастне дослідження шлунка, ультразвукове дослідження органів\r\nчеревної порожнини\r\nD. Фіброезофагогастродуоденоскопія, екскреторна урографія\r\nE. Сцинтиграфія печінки, фракційне дуоденальне зондування\r\n2. У хворого 45-ти років п'ятнадцятирічний анамнез виразкової хвороби дванадцятипалої\r\nкишки. Протягом 7-ми днів спостерігалося багаторазове блювання вмістом шлунка,\r\nзагальна слабкість. Пульс 100/хв., артеріальний тиск 90/50 мм рт.ст., тургор шкіри\r\nзнижений, пальпаторно живіт дещо болючий в епігастрії. На рентгенограмі велика\r\nкількість рідини у шлунку. Поставте діагноз:\r\nA. Пілоростеноз *\r\nB. Шлунково-кишкова кровотеча\r\nC. Гостра кишкова непрохідність\r\nD. Загострення виразкової хвороби дванадцятипалої кишки\r\nE. Гостре розширення шлунка\r\n3. Чоловіка 75-ти років доставлено до приймального відділення лікарні зі скаргами на\r\nінтенсивний біль у попереку, відсутність сечі протягом доби. Об'єктивно: артеріальний\r\nтиск 170/90 мм рт.ст., притуплення перкуторного звуку у надлобковій ділянці. Металевим\r\nкатетером виведено 750 мл сечі. Лабораторне дослідження сечі: сліди білку, лейкоцити\r\n10-12 екз. у полі зору, еритроцити 3-5 у препараті. Вміст сечовини у крові 7,8 ммоль/л,\r\nкреатиніну 0,11 ммоль/л. Назвіть причину відсутності сечі:\r\nA. Гостра затримка сечі внаслідок обструкції сечовипускника *\r\nB. Гострий гломерулонефрит\r\nC. Сечокам'яна хвороба\r\nD. Хронічна ниркова недостатність\r\nE. Гостра ниркова недостатність\r";
        public MainWindow()
        {
            InitializeComponent();
            coreFile = CoreFile.GetInstance(null, tr, this);
            coreFile.AI_Analize(new FilterPatterns(FilterPatterns.task, FilterPatterns.trueAns, FilterPatterns.falseAns, false), ParseType.LINEAR);
        }

        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            coreFile.Export();
        }

        private void UndoButton_OnClick(object sender, RoutedEventArgs e)
        {
            coreFile.Undo();
        }

        private void RedoButton_OnClick(object sender, RoutedEventArgs e)
        {
            coreFile.Redo();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string savng = coreFile.Encode();
            File.WriteAllText(Path.Combine(SpecialDirectories.Desktop, "sv.json"), savng);
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            string json = File.ReadAllText(Path.Combine(SpecialDirectories.Desktop, "sv.json"));
            coreFile = CoreFile.Decode(json);
        }
    }
}
