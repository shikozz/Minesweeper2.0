using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using saperdun.Properties;

namespace saperdun
{
    /// <summary>
    /// Тот кто узрит сие наследие - знай, что тут собрана кладезь самых разных решений. И парсинг XML-подобных строк.
    /// И работа с enum. И динамическое создание элементов интерфейса с подгонкой размеров. И работа с цветовыми темами и их переключением.
    /// И работа с шифрованием данных. И работа с потоками. И многоуровневый вызов. И рекурсивные методы...
    /// А еще тут поиграть можно =)
    /// </summary>
    public partial class MainWindow : Window
    {
        internal enum difficulties
        {
            beginner,
            intermediate,
            expert,
            professional,
            custom
        }
        internal enum paramType
        {
            score,
            time,
            date,
            result,
            shoppoints,
            bonusCodes,
            themesUnlocked,
            achievments,
            opraMinec,
            levelPlayer,
            currentExp,
            difficulty,
            openCharacters,
            powder_count,
            scrap_count,
            dirt_count,
            zakhar_level,
            evgen_level,
            leha_level
        }
        internal enum colorThemes
        {
            light,
            dark,
            golden,
            depths,
            skydive,
            cyberpunk

        }
        
        internal enum achievments
        {
            newJourney,
            localLegend,
            heroOfTheKingdom,
            hopeOfHumanity,
            speedster,
            worthy
        }

        private DispatcherTimer disableTimer;

        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ILoveMinthara");

        private int scrap_count = 0;
        private int powder_count = 0;
        private int flags_count = 0;

        public static string openCharacters = "";

        public static bool zakhar_player = false;
        public static int zakhar_chance = 3;
        public static int zakhar_level = 0;

        public static bool evgen_player = false;
        public static int evgen_chance = 5;
        public static int evgen_level = 0;

        public static bool leha_player = false;
        public static int leha_chance = 10;
        public static int leha_level = 0;

        public static bool worker_player = true;

        public static int loginStreak = 0;

        object buttonBackgroundBrush;
        object buttonFlaggedBackgroundBrush;
        object buttonRevealedBackgroundBrush;
        object buttonMineBackgroundBrush;
        object buttonAdjacentBackgroundBrush;
        object buttonSonarBackgroundBrush;
        object buttonForegroundBrush;
        object labelBackgroundBrush;
        object labelForegroundBrush;
        object labelWinBackgroundBrush;
        object labelWinForegroundBrush;
        object labelLoseBackgroundBrush;
        object labelLoseForegroundBrush;
        object buttonBoostHoverBackground;
        object transparentBackground;
        Style buttonStyle;
        Style labelStyle;
        Style textboxStyle;
        Style textblockStyle;
        Style txtBoxHeader;
        Style checkboxStyle;
        Style windowStyle;
        Style buttonGlowStyle;
        Style revertStyle;
        //Style datagridStyle;

        private int midTimeEasy =0;
        private int midCountEasy = 0;
        private string bestCompleteTimeEasy= "59:59";
        private int highestScoreEasy = 0;
        private int midTimeMid = 0;
        private int midCountMid = 0;
        private string bestCompleteTimeMid = "59:59";
        private int highestScoreMid = 0;
        private int midTimeHard = 0;
        private int midCountHard = 0;
        private string bestCompleteTimeHard = "59:59";
        private int highestScoreHard = 0;
        private int midTimeExpert = 0;
        private int midCountExpert = 0;
        private string bestCompleteTimeExpert = "59:59";
        private int highestScoreExpert = 0;
        private int totalGamesPlayed = 0;
        private int totalLegacyPlayed = 0;
        private int totalGamesWins = 0;
        private int totalGamesDraws = 0;
        private int lvlCount = 0;
        private int expCounter = 0;
        private int passiveBonus = 0;
        private string debugstring="";
        private difficulties selectedDiff = difficulties.intermediate;
        private string bonusCodesStr = "";
        private string themesUnlocked = "";
        private string achievmentsStr = "";
        string gameResult = "";
        private double fontSize = 0;
        private double startFontSize = 0;
        private double boostFontSize = 0;
        private double boost2FontSize = 0;
        private int rows = 50;
        private int columns = 50;
        private int mineCount = 10;
        public int mainHeight = 450;
        public static int shoppoints = 0;
        private int openedMineCount = 0;
        private int openRandomMinePrice = 250;
        private int openedTileCount = 0;
        private int openRandomTilePrice = 150;
        private int openCloseSafePrice = 1000;
        private bool[,] minefield; 
        private int[,] adjacentMines;
        private bool[,] revealed;
        private bool[,] flagged;
        private bool[,] shopRevealed;
        private bool[,] evgenTries;
        private int score;
        private Button[,] buttons;
        private bool gameOver = false;
        private int bufRevx = 0;
        private int bufRevy = 0;
        private int boostX = 0;
        private int boostY = 0;
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private TimeSpan leftTime;
        private bool sonarWorked = false;
        private bool scoreNeedToWrite = false;
        private bool countNoMoreScore = false;
        private bool doubleDown = false;
        private bool btnLeft = false;
        private bool btnRight = false;
        private bool needToCountShoppoints = false;
        private MouseEventArgs bufMEA;
        private RoutedEventArgs bufREA;
        private Button buffButton;
        private Random rnd;
        private bool scanBoostActive = false;
        private bool bonusCodeEnter = false;
        private bool speedBoost = false;
        private bool autoLose = false;
        private int steps = 0;
        private bool cbGlow = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            disableTimer = new DispatcherTimer();
            disableTimer.Interval = TimeSpan.FromMilliseconds(150);
            disableTimer.Tick += DisableTimer_Tick;
            Image characters = new Image();
            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/e238.png", UriKind.RelativeOrAbsolute));
            characters.Source = bitmap;
            characters.Width = 15;
            characters.Height = 15;
            characterChoose.Content = characters;
            mid.IsChecked = true;
            rnd = new Random();
        }

        private bool isDivisible(int x, int n)
        {
            return (x % n)==0;
        }

        private void paintLabels()
        {
            boostLabel.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, boostFontSize) } };
            txtSkydiveTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtScifiTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtAutumnTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtDepthsTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            scrapTxt.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            powderTxt.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            flagsTxt.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtCyberpunkTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGoldenTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBestresults.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            //txtAchi.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtCursor.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGrassyTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtMidTime.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtMidTimeShow.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtSpeedGame.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBesResutlForDif.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBestScore.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBestScoreShow.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBestTime.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtBestTimeShow.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtWR.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtEnableNewScores.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtEnableOldScores.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtWinrate.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtLevel.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGamesPlayed.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtNewScores.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGamesPlayedCount.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGamesPlayedLegacy.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtGamesPlayedCountLegacy.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtLevel.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtTheme.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            txtStyles.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, boost2FontSize) } };
            txtThemes.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            TitleText.Style = new Style(typeof(TextBlock), txtBoxHeader) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            textOpenRandomMine.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            textOpenRandomSafe.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            enhancedSonar.Style = new Style(typeof(TextBlock), textblockStyle) { Setters = { new Setter(TextBlock.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBlock.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            bonusCodeLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, boost2FontSize) } };
            bonusTextBox.Style = new Style(typeof(TextBox), textboxStyle) { Setters = { new Setter(TextBox.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBox.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            pointsLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            shoppointL.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            scoreTextLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            scoreLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            minesLeftLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            minesLeft.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            easy.Style = new Style(typeof(CheckBox), checkboxStyle) { Setters = { new Setter(CheckBox.FontSizeProperty, fontSize) } };
            mid.Style = new Style(typeof(CheckBox), checkboxStyle) { Setters = { new Setter(CheckBox.FontSizeProperty, fontSize) } };
            expert.Style = new Style(typeof(CheckBox), checkboxStyle) { Setters = { new Setter(CheckBox.FontSizeProperty, fontSize) } };
            hard.Style = new Style(typeof(CheckBox), checkboxStyle) { Setters = { new Setter(CheckBox.FontSizeProperty, fontSize) } };
            custom.Style = new Style(typeof(CheckBox), checkboxStyle) { Setters = { new Setter(CheckBox.FontSizeProperty, fontSize) } };
            rowsLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            rowsBox.Style = new Style(typeof(TextBox), textboxStyle) { Setters = { new Setter(TextBox.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBox.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            colsLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            colBox.Style = new Style(typeof(TextBox), textboxStyle) { Setters = { new Setter(TextBox.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBox.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            minesLabel.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            recMine.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            minesBox.Style = new Style(typeof(TextBox), textboxStyle) { Setters = { new Setter(TextBox.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(TextBox.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(TextBlock.FontSizeProperty, fontSize) } };
            startButton.Style = new Style(typeof(Button), buttonStyle) { Setters = {new Setter(Button.FontSizeProperty, startFontSize) } };
            patchNotesBtn.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.FontSizeProperty, fontSize) } };
            btnStart.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.FontSizeProperty, fontSize) } };
            shop.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.FontSizeProperty, fontSize) } };
            activateCode.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.FontSizeProperty, fontSize) } };
        }

        private void DisableTimer_Tick(object sender, EventArgs e)
        {
            if (scanBoostActive)
            {
                openCloseSafeTask();
                return;
            }
            disableTimer.Stop();
            doubleDown = false;
            if(btnRight)
            {
                Button_RMB(disableTimer.Tag, bufMEA);
            }
            if(btnLeft)
            {
                Button_Click(disableTimer.Tag, bufREA);
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(speedBoost)
            {
                leftTime = leftTime-(TimeSpan.FromMilliseconds(timer.Interval.TotalMilliseconds));
            }
            elapsedTime = elapsedTime.Add(TimeSpan.FromMilliseconds(timer.Interval.TotalMilliseconds));
            updateTimerDisplay();
        }

        private void updateTimerDisplay()
        {
            if (speedBoost)
            {
                time.Content = leftTime.ToString(@"mm\:ss");
                if(leftTime == TimeSpan.ParseExact("00:00", "mm\\:ss", null))
                {
                    EndGame(false);
                }
                switch (selectedDiff)
                {
                    case difficulties.beginner:
                        if(leftTime <= TimeSpan.ParseExact("00:30", "mm\\:ss", null))
                        {
                            autoLose = true;
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelLoseBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelLoseForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        else
                        {
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelWinBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelWinForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        break;
                    case difficulties.intermediate:
                        if(leftTime <= TimeSpan.ParseExact("01:00", "mm\\:ss", null))
                        {
                            autoLose = true;
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelLoseBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelLoseForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        else
                        {
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelWinBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelWinForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        break;
                    case difficulties.expert:
                        if(leftTime <= TimeSpan.ParseExact("01:00", "mm\\:ss", null))
                        {
                            autoLose = true;
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelLoseBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelLoseForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        else
                        {
                            autoLose = true;
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelWinBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelWinForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        break;
                    case difficulties.professional:
                        if(leftTime <= TimeSpan.ParseExact("01:30", "mm\\:ss", null))
                        {
                            autoLose = true;
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelLoseBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelLoseForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        else
                        {
                            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelWinBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelWinForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
                        }
                        break;
                }
            }
            else
            {
                time.Content = elapsedTime.ToString(@"mm\:ss");
            }
        }
        private void InitializeGame()
        {
            // Clear any previous game state
            GameGrid.Children.Clear();
            gameOver = false;
            minesLeft.Content = mineCount.ToString();
            minefield = new bool[rows, columns];
            adjacentMines = new int[rows, columns];
            buttons = new Button[rows, columns];
            revealed = new bool[rows, columns];
            flagged = new bool[rows, columns];
            shopRevealed= new bool[rows, columns];
            evgenTries = new bool[rows, columns];
            // Initialize Grid (UniformGrid)
            GameGrid.Rows = rows;
            GameGrid.Columns = columns;

            // Place Mines
            PlaceMines();

            // Calculate Adjacent Mines
            CalculateAdjacentMines();

            // Create Buttons
            CreateButtons();
        }

        private void PlaceMines()
        {
            Random random = new Random();
            int minesPlaced = 0;
            while (minesPlaced < mineCount)
            {
                int row = random.Next(rows);
                int col = random.Next(columns);
                if (!minefield[row, col])
                {
                    minefield[row, col] = true;
                    minesPlaced++;
                }
            }
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (!minefield[row, col])
                    {
                        int count = 0;
                        // Check adjacent cells
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (i == 0 && j == 0) continue; // Skip the current cell

                                int adjRow = row + i;
                                int adjCol = col + j;

                                if (adjRow >= 0 && adjRow < rows && adjCol >= 0 && adjCol < columns && minefield[adjRow, adjCol])
                                {
                                    count++;
                                }
                            }
                        }
                        adjacentMines[row, col] = count;
                    }
                }
            }
        }

        private void CreateButtons()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Button button = new Button();
                    button.Content = ""; // Initially hidden
                    button.Tag = new Tuple<int, int>(row, col); // Store row/col information in the button's Tag
                    button.Click += Button_Click;
                    button.MouseRightButtonDown += Button_RMB;
                    button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush), new Setter(Button.ForegroundProperty, (Brush)buttonBackgroundBrush) } };
                    buttons[row, col] = button;
                    GameGrid.Children.Add(button);
                    button.UpdateLayout();
                }
            }
            ThemeButtonFromWindows();
            //App.setGlowEffect(Settings.Default.Theme);
        }


        private void Button_RMB(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                clearSonar();
                Button button = (Button)sender;
                Tuple<int, int> position = (Tuple<int, int>)button.Tag;
                int row = position.Item1;
                int col = position.Item2;
                //lets try. FUCKING WORKED
                if (!doubleDown)
                {
                    if (!btnRight)
                    {
                        doubleDown = true;
                        btnRight = true;
                        buffButton = button;
                        disableTimer.Tag = button;
                        bufMEA = e;
                        boostX = row;
                        boostY = col;
                        disableTimer.Start();
                    }
                    else
                    {
                        CheckForWin();
                        if (flagged[row,col])
                        {
                            if (minefield[row,col])
                            {
                                if (!shopRevealed[row,col]) openedMineCount--;
                                score -= 20;
                            }
                            else
                            {
                                score += 15;
                            }
                            flagged[row, col] = false;
                            button.Content = "";
                            if (ThemeManager.CurrentTheme == "CyberpunkTheme" && cbGlow) button.Style = buttonGlowStyle;
                            else button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
                            minesLeft.Content = (Convert.ToInt32(minesLeft.Content) + 1).ToString();
                            flagged[row, col] = false;
                        }
                        else
                        {
                            if (!revealed[row, col])
                            {
                                if (minefield[row, col])
                                {
                                    score += 20;
                                    if (!shopRevealed[row,col])openedMineCount++;
                                    CheckForWin();
                                }
                                else
                                {
                                    score -= 20;
                                }
                                minesLeft.Content = (Convert.ToInt32(minesLeft.Content) - 1).ToString();
                                Image flag = new Image();
                                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                                flag.Source = bitmap;
                                flag.Width = 20;
                                flag.Height = 20;
                                button.Content = flag;
                                button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonFlaggedBackgroundBrush) } };
                                flagged[row, col] = true;
                            }
                        }
                        btnRight = false;
                    }
                }
                else
                {
                    //some double click code
                    if(buffButton==button)
                    {
                        clearSonar();
                        boostX = row;
                        boostY = col;
                        if (revealed[row, col])
                        {
                            int chance = rnd.Next(100);
                            if (evgen_player&& chance <= evgen_chance * evgen_level && !evgenTries[row,col])
                            {
                                openCloseSafeTask();
                                evgenTries[row, col] = true;
                                shoppoints += openCloseSafePrice;
                                shoppointL.Content = shoppoints;
                            }
                            else
                            {
                                sonar(row, col);
                            }                          
                        }
                        doubleDown = false;
                        disableTimer.Stop();
                        btnLeft = false;
                        btnRight = false;
                    }
                }
                CheckForWin();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver) return; // Don't allow clicks after the game is over
            clearSonar();
            Button button = (Button)sender;
            Tuple<int, int> position = (Tuple<int, int>)button.Tag;
            int row = position.Item1;
            int col = position.Item2;
            if (!doubleDown)
            {
                if (!btnLeft)
                {
                    doubleDown = true;
                    btnLeft = true;
                    buffButton = button;
                    disableTimer.Tag = button;
                    bufREA = e;
                    boostX = row;
                    boostY = col;
                    disableTimer.Start();
                }
                else
                {
                    steps++;
                    if (minefield[row, col])
                    {
                        if (!flagged[row, col])
                        {
                            int chance = rnd.Next(100);
                            if (zakhar_player&& chance <= zakhar_chance*zakhar_level)
                            {
                                minesLeft.Content = (Convert.ToInt32(minesLeft.Content) - 1).ToString();
                                Image flag = new Image();
                                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                                flag.Source = bitmap;
                                flag.Width = 20;
                                flag.Height = 20;
                                button.Content = flag;
                                button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonFlaggedBackgroundBrush) } };
                                flagged[row, col] = true;
                                if(zakhar_level==5)
                                {
                                    int chance3 = rnd.Next(100);
                                    if (chance3 <= 50)
                                    {
                                        openRandomMineLabel_Click(null, null);
                                        shoppoints += 250;
                                        shoppointL.Content = shoppoints;
                                    }
                                    else
                                    {
                                        openRandomSafeLabel_Click(null, null);
                                        shoppoints += 150;
                                        shoppointL.Content = shoppoints;
                                    }
                                }
                            }
                            else
                            {
                                // Game Over - Mine Hit!
                                Image bomb = new Image();
                                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/bimba.png"));
                                bomb.Source = bitmap;
                                bomb.Width = 20;
                                bomb.Height = 20;
                                button.Content = bomb;
                                button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonMineBackgroundBrush) } };
                                if (needToCountShoppoints) EndGame(false);
                            }
                        }
                    }
                    else
                    {
                        // Display Adjacent Mines Count
                        if (!flagged[row,col])
                        {
                            tryToReveal(row, col);
                            CheckForWin();
                        }          
                    }
                    btnLeft = false;
                }
            }
            else
            {
                //some double click code
                if(buffButton==button)
                {
                    clearSonar();
                    boostX = row;
                    boostY = col;
                    if (revealed[row, col])
                    {
                        int chance = rnd.Next(100);
                        if (evgen_player && chance <= evgen_chance * evgen_level && !evgenTries[row, col])
                        {
                            openCloseSafeTask();
                            evgenTries[row, col] = true;
                            shoppoints += openCloseSafePrice;
                            shoppointL.Content = shoppoints;
                        }
                        else
                        {
                            sonar(row, col);
                        }
                    }
                    doubleDown = false;
                    disableTimer.Stop();
                    btnLeft = false;
                    btnRight = false;
                }
            }
            CheckForWin();
        }

        private void tryToReveal(int row, int col)
        {
            if (row < 0 || row >= rows || col < 0 || col >= columns) return;
            if (revealed[row, col]) return;
            if (minefield[row, col]) return;
            if (adjacentMines[row, col] > 0)
            {
                if (buttons[row, col].Content.ToString() == "!")
                {
                    minesLeft.Content = (Convert.ToInt32(minesLeft.Content) + 1).ToString();
                }
                buttons[row, col].Content = adjacentMines[row, col].ToString();
                buttons[row, col].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.ForegroundProperty, (Brush)buttonForegroundBrush) } };
                buttons[row, col].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonAdjacentBackgroundBrush) } };
                revealed[row, col] = true;
                if(!countNoMoreScore)score += 5;
                CheckForWin();
                return;
            }
            if (buttons[row, col].Content.ToString() == "!")
            {
                minesLeft.Content = (Convert.ToInt32(minesLeft.Content) + 1).ToString();
            }
            buttons[row, col].Content = "";
            buttons[row, col].IsEnabled = false;
            buttons[row, col].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
            if(!countNoMoreScore)score += 1;
            revealed[row, col] = true;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    tryToReveal(row + i, col + j);
                }
            }
            CheckForWin();
        }

        private void sonar(int row,int col)
        {
            int countFlagged = 0;
            int countAllFlagged = 0;
            for(int i = row-1; i<=row+1;i++)
            {
                for(int j = col-1; j<=col+1;j++)
                {
                    if (i >= 0&& i< rows && j>=0 &&j<columns)
                    {
                        if ((i != row || j != col) && !revealed[i, j] && !flagged[i,j])
                        {
                            buttons[i, j].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonSonarBackgroundBrush) } };
                            bufRevx = row;
                            bufRevy = col;
                            sonarWorked = true;
                        }
                    }
                }
            }
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < rows && j >= 0 && j < columns)
                    {
                        if ((i != row || j != col) && (minefield[i,j]&&flagged[i,j]))
                        {
                            countFlagged++;
                        }
                        if ((i != row || j != col) && (flagged[i, j]))
                        {
                            countAllFlagged++;
                        }
                    }
                }
            }
            if (countAllFlagged == adjacentMines[row,col])
            {
                if (adjacentMines[row, col] == countFlagged)
                {
                    for (int i = row - 1; i <= row + 1; i++)
                    {
                        for (int j = col - 1; j <= col + 1; j++)
                        {
                            if (i >= 0 && i < rows && j >= 0 && j < columns)
                            {
                                if ((i != row || j != col) && !revealed[i, j] && !flagged[i, j])
                                {
                                    tryToReveal(i, j);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = row - 1; i <= row + 1; i++)
                    {
                        for (int j = col - 1; j <= col + 1; j++)
                        {
                            if (i >= 0 && i < rows && j >= 0 && j < columns)
                            {
                                if ((i != row || j != col) && minefield[i, j] && !flagged[i, j])
                                {
                                    // Game Over - Mine Hit!
                                    Image bomb = new Image();
                                    BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/bimba.png"));
                                    bomb.Source = bitmap;
                                    bomb.Width = 20;
                                    bomb.Height = 20;
                                    buttons[i, j].Content = bomb;
                                    buttons[i, j].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonMineBackgroundBrush) } };
                                    if(needToCountShoppoints)EndGame(false);
                                    break;
                                }
                            }
                        }
                    }
                }       
            }
            else if (countAllFlagged > adjacentMines[row,col])
            {
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = col - 1; j <= col + 1; j++)
                    {
                        if (i >= 0 && i < rows && j >= 0 && j < columns)
                        {
                            if ((i != row || j != col) && minefield[i, j] && !flagged[i, j])
                            {
                                // Game Over - Mine Hit!
                                Image bomb = new Image();
                                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/bimba.png"));
                                bomb.Source = bitmap;
                                bomb.Width = 20;
                                bomb.Height = 20;
                                buttons[i, j].Content = bomb;
                                buttons[i, j].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonMineBackgroundBrush) } };
                                if(needToCountShoppoints)EndGame(false);
                                break;
                            }
                        }
                    }
                }
            }
            CheckForWin();
        }

        private void clearSonar()
        {
            if(sonarWorked)
            {
                int row = bufRevx;
                int col = bufRevy;
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = col - 1; j <= col + 1; j++)
                    {
                        if (i >= 0 && i < rows && j >= 0 && j < columns)
                        {
                            if ((i != row || j != col) && !revealed[i,j] && !flagged[i, j])
                            {
                                if (ThemeManager.CurrentTheme == "CyberpunkTheme" && cbGlow) buttons[i,j].Style = buttonGlowStyle;
                                else buttons[i, j].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
                            }
                        }
                    }
                }
                sonarWorked = !sonarWorked;
            }
        }

        private void checkAchievments(achievments ac)
        {
            switch(ac)
            {
                case achievments.newJourney:
                    if(!achievmentsStr.Contains("newJourney"))
                    {
                        achievmentsStr += ".newJourney.";
                        MSGBox.Show("Получено достижение \"Новое приклюение\"! " +
                            "\nОткрыта панель \"Бусты\"");
                    }
                    break;
                case achievments.localLegend:
                    if (!achievmentsStr.Contains("localLegend"))
                    {
                        achievmentsStr += ".localLegend.";
                        MSGBox.Show("Получено достижение \"Местная легенда\"! " +
                            "\nОткрыта панель \"Бонус-коды\" в разделе \"Бусты\"");
                    }
                    if (!achievmentsStr.Contains("newJourney"))
                    {
                        achievmentsStr += ".newJourney.";
                        MSGBox.Show("Получено достижение \"Новое приклюение\"! " +
                            "\nОткрыта панель \"Бусты\"");
                    }
                    break;
                case achievments.heroOfTheKingdom:
                    if (!achievmentsStr.Contains("heroOfTheKingdom"))
                    {
                        achievmentsStr += ".heroOfTheKingdom.";
                        MSGBox.Show("Получено достижение \"Герой королевства\"! " +
                            "\nОткрыта панель \"Оформление\" в разделе \"Бусты\"");
                    }
                    if (!achievmentsStr.Contains("localLegend"))
                    {
                        achievmentsStr += ".localLegend.";
                        MSGBox.Show("Получено достижение \"Местная легенда\"! " +
                            "\nОткрыта панель \"Бонус-коды\" в разделе \"Бусты\"");
                    }
                    if (!achievmentsStr.Contains("newJourney"))
                    {
                        achievmentsStr += ".newJourney.";
                        MSGBox.Show("Получено достижение \"Новое приклюение\"! " +
                            "\nОткрыта панель \"Бусты\"");
                    }
                    break;
                case achievments.hopeOfHumanity:
                    if (!achievmentsStr.Contains("hopeOfHumanity"))
                    {
                        achievmentsStr += ".hopeOfHumanity.";
                        MSGBox.Show("Получено достижение \"Надежда человечества\"! " +
                            "\nОткрыто постоянное увеличение очков");
                    }

                    if (!achievmentsStr.Contains("heroOfTheKingdom"))
                    {
                        achievmentsStr += ".heroOfTheKingdom.";
                        MSGBox.Show("Получено достижение \"Герой королевства\"! " +
                            "\nОткрыта панель \"Оформление\" в разделе \"Бусты\"");
                    }
                    if (!achievmentsStr.Contains("localLegend"))
                    {
                        achievmentsStr += ".localLegend.";
                        MSGBox.Show("Получено достижение \"Местная легенда\"! " +
                            "\nОткрыта панель \"Бонус-коды\" в разделе \"Бусты\"");
                    }
                    if (!achievmentsStr.Contains("newJourney"))
                    {
                        achievmentsStr += ".newJourney.";
                        MSGBox.Show("Получено достижение \"Новое приклюение\"! " +
                            "\nОткрыта панель \"Бусты\"");
                    }
                    break;
                case achievments.speedster:
                    if(!achievmentsStr.Contains("speedster"))
                    {
                        achievmentsStr += ".speedster.";
                        MSGBox.Show("Получено достижение \"Спидстер\"" +
                            "\nДоступен новый режим игры в меню \"Бусты\"!");
                    }
                    break;
                case achievments.worthy:
                    if(!achievmentsStr.Contains("worthy"))
                    {
                        achievmentsStr += ".worthy.";
                        MSGBox.Show("Получено достижение \"Достойный\"" +
                            "\nОткрыта возможность получения персонажей!");
                    }
                    break;
            };
            checkOpenedAchievments();
        }

        private void checkOpenedAchievments()
        {
            if (achievmentsStr.Contains("newJourney"))
            {
                shop.Visibility = Visibility.Visible;
            }else
            {
                shop.Visibility = Visibility.Hidden;
            }
            if(achievmentsStr.Contains("localLegend"))
            {
                BonusCodesPanel.Visibility = Visibility.Visible;
            }
            else
            {
                BonusCodesPanel.Visibility=Visibility.Hidden;
            }
            if(achievmentsStr.Contains("heroOfTheKingdom"))
            {
                VisualPanel.Visibility = Visibility.Visible;
            }
            else
            {
                VisualPanel.Visibility = Visibility.Hidden;
            }
            if(achievmentsStr.Contains("hopeOfHumanity"))
            {
                passiveBonus = 1000;
            }
            if(achievmentsStr.Contains("speedster"))
            {
                speedGamePanel.Visibility = Visibility.Visible;
            }
            else
            {
                speedGamePanel.Visibility = Visibility.Collapsed;
            }
            if(achievmentsStr.Contains("worthy"))
            {
                characterChoose.Visibility = Visibility.Visible;
                matPanel.Visibility = Visibility.Visible;
            }
            else
            {
                characterChoose.Visibility = Visibility.Collapsed;
                matPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void EndGame(bool win)
        {
            if (worker_player)
            {
                score += (score / 100) * 3;
            }
            if(leha_player)
            {
                int chance = rnd.Next(0-leha_chance,leha_chance);
                if(chance+leha_level <leha_chance)
                {
                    score += (score / 100) * chance + leha_level;
                }
                else if(chance+leha_level>leha_chance)
                {
                    score += (score / 100) * chance + leha_level;
                }
            }
            speedGameToggleButton.IsEnabled = true;
            time.Content = elapsedTime.ToString(@"mm\:ss");
            ThemeButtonFromWindows();
            if (win)
            {
                dayLoginCheck();
                TimeSpan ts2 = TimeSpan.ParseExact(elapsedTime.ToString(@"mm\:ss"), "mm\\:ss", null);
                TimeSpan ts1 = TimeSpan.ParseExact("00:01", "mm\\:ss", null); ;
                switch (selectedDiff)
                {
                    case difficulties.beginner:
                        ts1 = TimeSpan.ParseExact("00:30", "mm\\:ss", null);
                        break;
                    case difficulties.intermediate:
                        ts1 = TimeSpan.ParseExact("02:00", "mm\\:ss", null);
                        break;
                    case difficulties.expert:
                        ts1 = TimeSpan.ParseExact("03:30", "mm\\:ss", null);
                        break;
                    case difficulties.professional:
                        ts1 = TimeSpan.ParseExact("09:00", "mm\\:ss", null);
                        break;
                }
                if(ts2<ts1)
                {
                    checkAchievments(achievments.speedster);
                }
                gameResult = "victory";
                switch (selectedDiff)
                {
                    case difficulties.beginner:
                        if (speedBoost)
                        {
                            int mins = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[0]);
                            int secs= Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[1]);
                            secs += mins * 60;
                            if(secs<=60)
                            {
                                float result = (float)secs / 60;
                                float timeBooster = (float)1-result;
                                score += Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                                scrap_count += 2;
                                powder_count += 3;
                            }
                            else if(secs<=90)
                            {
                                float result = (float)secs / 90;
                                float timeBooster = (float)1 - result;
                                score = Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                            }
                            else
                            { 
                                score = 0;
                            }
                        }
                        shoppoints += score / 4;
                        shoppoints += passiveBonus;
                        checkAchievments(achievments.newJourney);
                        scrap_count += rnd.Next(3,7);
                        powder_count += rnd.Next(7,13);
                        if(leha_player&&leha_level==5)
                        {
                            scrap_count++;
                            powder_count += 2;
                        }
                        break;
                    case difficulties.intermediate:
                        if(speedBoost)
                        {
                            int mins = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[0]);
                            int secs = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[1]);
                            secs += mins * 60;
                            if (secs <= 180)
                            {
                                float result = (float)secs / 180;
                                float timeBooster = (float)1 - result;
                                score += Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                                scrap_count += 4;
                                powder_count +=6;
                                flags_count += 1;
                                checkAchievments(achievments.worthy);
                            }
                            else if (secs <= 240)
                            {
                                float result = (float)secs / 240;
                                float timeBooster = (float)1 - result;
                                score = Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                            }
                            else
                            {
                                score = 0;
                            }
                        }
                        shoppoints += score / 3;
                        shoppoints += passiveBonus;
                        scrap_count += rnd.Next(6, 11);
                        powder_count += rnd.Next(10, 17);
                        checkAchievments(achievments.localLegend);
                        if (leha_player && leha_level == 5)
                        {
                            scrap_count+=3;
                            powder_count += 5;
                        }
                        break;
                    case difficulties.expert:
                        if(speedBoost)
                        {
                            int mins = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[0]);
                            int secs = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[1]);
                            secs += mins * 60;
                            if (secs <= 300)
                            {
                                float result = (float)secs / 300;
                                float timeBooster = (float)1 - result;
                                score += Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                                scrap_count += 5;
                                powder_count += 7;
                                flags_count += 1;
                                if (leha_player && leha_level == 5)
                                {
                                    flags_count++;
                                }
                            }
                            else if (secs <= 360)
                            {
                                float result = (float)secs / 360;
                                float timeBooster = (float)1 - result;
                                score = Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                            }
                            else
                            {
                                score = 0;
                            }
                        }
                        shoppoints += score / 2;
                        scrap_count += rnd.Next(16, 24);
                        powder_count += rnd.Next(16, 30);
                        shoppoints += passiveBonus;
                        flags_count += 1;
                        checkAchievments(achievments.heroOfTheKingdom);
                        if (leha_player && leha_level == 5)
                        {
                            scrap_count += 4;
                            powder_count += 9;
                        }
                        break;
                    case difficulties.professional:
                        if(speedBoost)
                        {
                            int mins = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[0]);
                            int secs = Convert.ToInt32(elapsedTime.ToString(@"mm\:ss").Split(':')[1]);
                            secs += mins * 60;
                            if (secs <= 630)
                            {
                                float result = (float)secs / 630;
                                float timeBooster = (float)1 - result;
                                score += Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                                scrap_count += 8;
                                powder_count += 10;
                                flags_count += 2;
                                if (leha_player && leha_level == 5)
                                {
                                    scrap_count += 5;
                                    powder_count += 14;
                                }
                            }
                            else if (secs <= 720)
                            {
                                float result = (float)secs / 720;
                                float timeBooster = (float)1 - result;
                                score = Convert.ToInt32(score * timeBooster);
                                Console.WriteLine(score);
                            }
                            else
                            {
                                score = 0;
                            }
                        }
                        shoppoints += score;
                        shoppoints += passiveBonus;
                        scrap_count += rnd.Next(25, 35);
                        powder_count += rnd.Next(32, 47);
                        flags_count += 2;
                        if (leha_player && leha_level == 5)
                        {
                            scrap_count += 9;
                            powder_count += 18;
                            flags_count++;
                        }
                        checkAchievments(achievments.hopeOfHumanity);
                        if (!bonusCodesStr.Contains("киберпсих"))
                        {
                            MSGBox.Show("Доступен новый бонус-код! \n\"киберпсих\"");
                        }
                        break;
                    case difficulties.custom:
                        scoreLabel.Content = 1000;
                        score = 1000;
                        shoppoints += passiveBonus;
                        break;
                }
                scrapTxt.Text = "Част.: "+scrap_count;
                powderTxt.Text = "Пыл.: " + powder_count;
                flagsTxt.Text = "Флаж.: " + flags_count;
            }
            else
            {
                if (autoLose) score = 0;
                switch (selectedDiff)
                {
                    case difficulties.beginner:
                        if (steps <= 3) gameResult = "draw";
                        else gameResult = "defeat";
                        shoppoints += score / 8;
                        if (leha_player && leha_level == 5 && gameResult == "defeat")
                        {
                            scrap_count += rnd.Next(1,3);
                            powder_count += rnd.Next(2,4);
                        }
                        break;
                    case difficulties.intermediate:
                        if (steps <= 5) gameResult = "draw";
                        else gameResult = "defeat";
                        shoppoints += score / 6;
                        if (leha_player && leha_level == 5 && gameResult == "defeat")
                        {
                            scrap_count += rnd.Next(2, 4);
                            powder_count += rnd.Next(3, 5);
                        }
                        break;
                    case difficulties.expert:
                        if (steps <= 7) gameResult = "draw";
                        else gameResult = "defeat";
                        shoppoints += score / 4;
                        if (leha_player && leha_level == 5&&gameResult=="defeat")
                        {
                            scrap_count += rnd.Next(3, 5);
                            powder_count += rnd.Next(4, 6);
                        }
                        break;
                    case difficulties.professional:
                        if (steps <= 10) gameResult = "draw";
                        else gameResult = "defeat";
                        shoppoints += score / 2;
                        if (leha_player && leha_level == 5 && gameResult == "defeat")
                        {
                            scrap_count += rnd.Next(4, 6);
                            powder_count += rnd.Next(5, 7);
                        }
                        break;
                    case difficulties.custom:
                        shoppoints += 0;
                        break;
                }
            }
            
            if(leha_player&&leha_level==5&&gameResult!="draw")
            {
                int chance = rnd.Next(100);
                if(chance <=15)
                {
                    flags_count++;
                }
            }
            scanBoostActive = false;
            shoppointL.Content = shoppoints;
            scoreBoardUpdate();
            scoreNeedToWrite = false;
            gameOver = true;
            if (win)
            {
                timer.Stop();
                minesLeft.Visibility = Visibility.Collapsed;
                minesLeftLabel.Visibility = Visibility.Collapsed;
                scoreLabel.Foreground = Brushes.Green;
                scoreLabel.Background = Brushes.LightGreen;
                scoreLabel.Content = score.ToString()+" победа";
            }
            else
            {
                timer.Stop();              
                try
                {
                    btnStart.IsEnabled = false;
                    await revealAllMines();
                }
                finally
                {
                    btnStart.IsEnabled = true;
                    minesLeft.Visibility = Visibility.Collapsed;
                    minesLeftLabel.Visibility = Visibility.Collapsed;
                    scoreLabel.Foreground = Brushes.Red;
                    scoreLabel.Background = Brushes.Pink;
                    scoreLabel.Content = score.ToString() + " поражение";
                }
            }
            if (!achievmentsStr.Contains("worthy"))
            {
                scrap_count = 0;
                powder_count = 0;
                flags_count = 0;
            }
            startButton.Visibility = Visibility.Visible;
            startButton.IsEnabled = true;
            openRandomMineLabel.IsEnabled = false;
            openRandomMineLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
            openRandomSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
            openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
            openCloseSafeLabel.IsEnabled = false;
            openRandomSafeLabel.IsEnabled = false;
            needToCountShoppoints = false;
        }

        private void CheckForWin()
        {
            int revealedCount = 0;
            openedTileCount = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (revealed[row, col])
                    {
                        revealedCount++;
                        openedTileCount++;
                    }
                }
            }
            int countAllFlagged = 0;
            for (int roww = 0; roww < rows; roww++)
            {
                for (int coll = 0; coll < columns; coll++)
                {
                    if (minefield[roww, coll] && flagged[roww, coll])
                    {
                        countAllFlagged++;
                    }
                }
            }
            if((revealedCount)==(rows*columns)-mineCount)
            {
                if(needToCountShoppoints)
                {
                    score += (mineCount - countAllFlagged) * 20;
                    EndGame(true);
                }

            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            startButton.Visibility = Visibility.Collapsed;
            startButton.IsEnabled = false;
            scoreLabel.Content = "0";
            scoreLabel.Style = new Style(typeof(Label), labelStyle) { Setters = {new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush) } };
            minesLeftLabel.Visibility = Visibility.Visible;
            minesLeft.Visibility = Visibility.Visible;
            scoreNeedToWrite = true;
            countNoMoreScore = false;
            openRandomMineLabel.IsEnabled = true;
            openCloseSafeLabel.IsEnabled = true;
            openRandomMineLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
            openRandomSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
            openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
            time.Style = new Style(typeof(Label), labelStyle) { Setters = { new Setter(Label.BackgroundProperty, (Brush)labelBackgroundBrush), new Setter(Label.ForegroundProperty, (Brush)labelForegroundBrush), new Setter(Label.FontSizeProperty, fontSize) } };
            openedMineCount = 0;
            openedTileCount = 0;
            speedGameToggleButton.IsEnabled = false;
            autoLose = false;
            openRandomSafeLabel.IsEnabled = true;
            setDifficulty();
            InitializeGame();
            steps = 0;
            elapsedTime = TimeSpan.Zero;
            if(speedBoost)
            {
                switch (selectedDiff)
                {
                    case difficulties.beginner:
                        leftTime = TimeSpan.ParseExact("01:30", "mm\\:ss", null);
                        break;
                    case difficulties.intermediate:
                        leftTime = TimeSpan.ParseExact("04:00", "mm\\:ss", null);
                        break;
                    case difficulties.expert:
                        leftTime = TimeSpan.ParseExact("06:00", "mm\\:ss", null);
                        break;
                    case difficulties.professional:
                        leftTime = TimeSpan.ParseExact("12:00", "mm\\:ss", null);
                        break;
                }
                    
            }
            score = 0;
            updateTimerDisplay();
            timer.Start();
            needToCountShoppoints = true;
        }

        private void setDifficulty()
        {
            switch (selectedDiff)
            {
                case difficulties.beginner:
                    if(shopPanel.Visibility == Visibility.Visible)
                    {
                        this.Width -= 200;
                        mainPanel.Margin = new Thickness(0, 0, 0, 0);
                        shopPanel.Visibility = Visibility.Collapsed;                  
                    }
                    rows = 9;
                    columns = 9;
                    mineCount = 10;
                    this.Width = (columns * 32) + 220;
                    this.Height = (rows * 32) + 240;
                    GameGrid.Height = rows * 31;
                    GameGrid.Width = columns * 31;
                    startButton.HorizontalAlignment = HorizontalAlignment.Left;
                    startButton.Margin = new Thickness((GameGrid.Width/2)-(startButton.Width/2), 0, 0, 0);
                    break;
                case difficulties.intermediate:
                    if (shopPanel.Visibility == Visibility.Visible)
                    {
                        this.Width -= 200;
                        mainPanel.Margin = new Thickness(0, 0, 0, 0);
                        shopPanel.Visibility = Visibility.Collapsed;
                    }
                    rows = 16;
                    columns = 16;
                    mineCount = 40;
                    this.Width = (columns * 32) + 220;
                    this.Height = (rows * 32) + 140;
                    GameGrid.Height = rows * 31;
                    GameGrid.Width = columns * 31;
                    startButton.HorizontalAlignment = HorizontalAlignment.Left;
                    startButton.Margin = new Thickness((GameGrid.Width / 2) - (startButton.Width / 2), 0, 0, 0);
                    break;
                case difficulties.expert:
                    if (shopPanel.Visibility == Visibility.Visible)
                    {
                        this.Width -= 200;
                        mainPanel.Margin = new Thickness(0, 0, 0, 0);
                        shopPanel.Visibility = Visibility.Collapsed;
                    }
                    rows = 16;
                    columns = 30;
                    mineCount = 99;
                    this.Width = (columns * 32) + 220;
                    this.Height = (rows * 32) + 140;
                    GameGrid.Height = rows * 31;
                    GameGrid.Width = columns * 31;
                    startButton.HorizontalAlignment = HorizontalAlignment.Left;
                    startButton.Margin = new Thickness((GameGrid.Width / 2) - (startButton.Width / 2), 0, 0, 0);
                    break;
                case difficulties.professional:
                    if (shopPanel.Visibility == Visibility.Visible)
                    {
                        this.Width -= 200;
                        mainPanel.Margin = new Thickness(0, 0, 0, 0);
                        shopPanel.Visibility = Visibility.Collapsed;
                    }
                    rows = 30;
                    columns = 30;
                    mineCount = 225;
                    this.Width = (columns * 32) + 220;
                    this.Height = (rows * 32) + 120;
                    GameGrid.Height = rows * 31;
                    GameGrid.Width = columns * 31;
                    startButton.HorizontalAlignment = HorizontalAlignment.Left;
                    startButton.Margin = new Thickness((GameGrid.Width / 2) - (startButton.Width / 2), 0, 0, 0);
                    break;
                case difficulties.custom:
                    if (shopPanel.Visibility == Visibility.Visible)
                    {
                        this.Width -= 200;
                        mainPanel.Margin = new Thickness(0, 0, 0, 0);
                        shopPanel.Visibility = Visibility.Collapsed;
                    }
                    rows = Convert.ToInt32(rowsBox.Text);
                    columns = Convert.ToInt32(colBox.Text);
                    mineCount = Convert.ToInt32(minesBox.Text);
                    if (mineCount >= (rows * columns)) mineCount = (rows * columns) - 1;

                    if (920 / rows >= 20)
                    {
                        this.Height = 950;
                        GameGrid.Height = 900;
                        this.Width = ((950 / rows) * columns) + 220;
                        GameGrid.Width = (950 / rows) * columns;
                    }
                    else
                    {
                        this.Height = (rows * 20)+ 120;
                        GameGrid.Height = (rows * 20);
                        if(GameGrid.Height<620)
                        {
                            GameGrid.Height = 620;
                        }
                        this.Width = (columns * 20) + 220;
                        GameGrid.Width = (columns * 20);
                    }
                    startButton.HorizontalAlignment = HorizontalAlignment.Left;
                    startButton.Margin = new Thickness((GameGrid.Width / 2) - (startButton.Width / 2), 0, 0, 0);
                    break;
            }
            hideMenu.Margin = new Thickness(0, 0, 200, 0);
            characterChoose.Margin = new Thickness(0, 25, 200, 0);
        }
        private void scoreBoardUpdate()
        {
            if (scoreNeedToWrite&&!bonusCodeEnter)
            {
                expCounter += score;
                while (expCounter >= 1000)
                {
                    lvlCount++;
                    if (isDivisible(lvlCount, 5)) shoppoints += 500;
                    if (isDivisible(lvlCount, 10))
                    { 
                        shoppoints += 500; 
                        MSGBox.Show("Подздравляем с получением " + lvlCount + " уровня! Вы получили в награду 500 поинтов!"); 
                    }
                    else if (isDivisible(lvlCount, 50)) 
                    { 
                        shoppoints += 1500;
                        MSGBox.Show("Подздравляем с получением " + lvlCount + " уровня! Вы получили в награду 1500 поинтов!"); 
                    }
                    else if (isDivisible(lvlCount, 100)) 
                    { 
                        shoppoints += 6500; 
                        MSGBox.Show("Подздравляем с получением " + lvlCount + " уровня! Вы получили в награду 6500 поинтов!"); 
                    }
                    expCounter -= 1000;
                }
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exePath, "saperdun.config");
                EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                string contentPrev = File.ReadAllText(filePath);
                string[] contentPrevArr = contentPrev.Split('\n');
                contentPrev = "<mainbody shoppoint=\""+shoppoints.ToString()+"\" levelPlayer=\""+lvlCount+ "\" currentExp=\""+expCounter+"\" bonusCodes=\"" + bonusCodesStr+ "\" themesUnlocked=\""+themesUnlocked+ "\" " +
                    "achievments=\""+achievmentsStr+"\" scrap_count=\""+scrap_count+"\" powder_count=\""+powder_count+"\" dirt_count=\""+flags_count+"\" openCharacters=\""+openCharacters+"\"" +
                    " zakhar_level=\""+zakhar_level+"\" evgen_level=\""+evgen_level+"\" leha_level=\""+leha_level+"\"/>\n";
                for(int i =1; i<contentPrevArr.Count(); i++)
                {
                    if (contentPrevArr[i] != "") contentPrev += contentPrevArr[i]+"\n";
                }
                string content = "<LINE SCORE=\"" + score + "\" TIME=\"" + elapsedTime.ToString(@"mm\:ss") + "\" DATE=\"" + DateTime.Now + "\" result=\""+gameResult+"\" difficulty=\""+selectedDiff+"\"/>";
                File.WriteAllText(filePath, contentPrev + content);
                EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                scoreBoardLoad();
            }
            else
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exePath, "saperdun.config");
                EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                string contentPrev = File.ReadAllText(filePath);
                string[] contentPrevArr = contentPrev.Split('\n');
                contentPrev = "<mainbody shoppoint=\"" + shoppoints.ToString() + "\" levelPlayer=\""+lvlCount+"\" currentExp=\""+expCounter+"\" bonusCodes=\"" + bonusCodesStr + "\" themesUnlocked=\""+themesUnlocked+ "\" " +
                    "achievments=\""+achievmentsStr+ "\" scrap_count=\"" + scrap_count + "\" powder_count=\"" + powder_count + "\" dirt_count=\"" + flags_count + "\" openCharacters=\"" + openCharacters + "\"" +
                    " zakhar_level=\"" + zakhar_level + "\" evgen_level=\""+evgen_level+"\" leha_level=\""+leha_level+"\"/>\n";
                for (int i = 1; i < contentPrevArr.Count(); i++)
                {
                    if (contentPrevArr[i]!="")contentPrev += contentPrevArr[i] + "\n";
                }
                File.WriteAllText(filePath, contentPrev);

                EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                scoreBoardLoad();
                bonusCodeEnter = false;
            }
        }
        private void scoreBoardLoad()
        {
            totalGamesWins = 0;
            midCountEasy= 0;
            midCountExpert = 0;
            midCountHard = 0;
            totalGamesPlayed = 0;
            totalLegacyPlayed = 0;
            totalGamesDraws = 0;
            midCountMid = 0;
            midTimeEasy = 0;
            midTimeExpert = 0;
            midTimeHard = 0;
            midTimeMid = 0;
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(exePath, "saperdun.config");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            else
            {
                if (File.ReadAllText(filePath).Contains("mainbody"))EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                EncryptDecryptFile(filePath);
                debugstring = File.ReadAllText(filePath);
                string scoreStr = File.ReadAllText(filePath);
                if (scoreStr!=null) 
                {
                    try
                    {
                        shoppoints = Convert.ToInt32(cutFromNode(scoreStr, paramType.shoppoints));                     
                    }
                    catch
                    {
                        shoppoints = 0;
                    }
                    bool needToCountLevels = false;
                    if (cutFromNode(scoreStr, paramType.levelPlayer) == "") needToCountLevels = true;
                    shoppointL.Content = shoppoints;
                    string[] scores = scoreStr.Split('\n');
                    List<DataItem> dataList = new List<DataItem>();
                    for (int i=1; i<scores.Length; i++)
                    {
                        if (scores[i].Length>0)
                        {
                            int score = Convert.ToInt32(cutFromNode(scores[i], paramType.score));
                            DataItem item = new DataItem();
                            item.sc = Convert.ToInt32(cutFromNode(scores[i], paramType.score));
                            string timeStr = cutFromNode(scores[i], paramType.time);
                            item.time = timeStr;
                            string date = cutFromNode(scores[i], paramType.date);
                            if(date!="")
                            {
                                item.date= date.Remove(date.IndexOf(' '));
                            }             
                            dataList.Add(item);

                            if(needToCountLevels)
                            {
                                expCounter += score;
                                while (expCounter >= 1000)
                                {
                                    lvlCount++;
                                    expCounter -= 1000;
                                }
                            }
                            string gameresult = cutFromNode(scores[i], paramType.result);
                            string format = "dd.MM.yyyy H:mm:ss";
                            DateTime patchDate = DateTime.ParseExact("04.04.2025 00:00:00", format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            DateTime gameDate = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            if (gameresult!=""&&(gameDate>patchDate))
                            {
                                totalGamesPlayed++;
                                if(gameresult=="victory")
                                {
                                    totalGamesWins++;
                                }
                                else if(gameresult=="draw")
                                {
                                    totalGamesDraws++;
                                }
                            }
                            else if(date!="")
                            {
                                totalLegacyPlayed++;
                            }
                            TimeSpan ts2 = TimeSpan.ParseExact(timeStr,"mm\\:ss",null);
                            TimeSpan ts1 = new TimeSpan();
                            string gameDifficulty = cutFromNode(scores[i], paramType.difficulty);
                            if(gameDifficulty!=""&&gameresult=="victory")
                            {
                                switch(gameDifficulty)
                                {
                                    case "beginner":
                                        ts1 = TimeSpan.ParseExact(bestCompleteTimeEasy, "mm\\:ss", null);
                                        if (score>highestScoreEasy)
                                        {
                                            highestScoreEasy = score;
                                        }
                                        if(ts2<ts1)
                                        {
                                            bestCompleteTimeEasy = timeStr;
                                        }
                                        int minutes = Convert.ToInt32(timeStr.Split(':')[0]);
                                        int seconds = Convert.ToInt32(timeStr.Split(':')[1]);
                                        seconds += minutes * 60;
                                        midCountEasy++;
                                        midTimeEasy += seconds;
                                        break;
                                    case "intermediate":
                                        ts1 = TimeSpan.ParseExact(bestCompleteTimeMid, "mm\\:ss", null);
                                        if (score>highestScoreMid)
                                        {
                                            highestScoreMid = score;
                                        }
                                        if (ts2 < ts1)
                                        {
                                            bestCompleteTimeMid = timeStr;
                                        }
                                        int minutes1 = Convert.ToInt32(timeStr.Split(':')[0]);
                                        int seconds1 = Convert.ToInt32(timeStr.Split(':')[1]);
                                        seconds1 += minutes1 * 60;
                                        midCountMid++;
                                        midTimeMid += seconds1;
                                        break;
                                    case "expert":
                                        ts1 = TimeSpan.ParseExact(bestCompleteTimeHard, "mm\\:ss", null);
                                        if (score > highestScoreHard)
                                        {
                                            highestScoreHard = score;
                                        }
                                        if (ts2 < ts1)
                                        {
                                            bestCompleteTimeHard = timeStr;
                                        }
                                        int minutes2 = Convert.ToInt32(timeStr.Split(':')[0]);
                                        int seconds2 = Convert.ToInt32(timeStr.Split(':')[1]);
                                        seconds2 += minutes2 * 60;
                                        midCountHard++;
                                        midTimeHard += seconds2;
                                        break;
                                    case "professional":
                                        ts1 = TimeSpan.ParseExact(bestCompleteTimeExpert, "mm\\:ss", null);
                                        if (score > highestScoreExpert)
                                        {
                                            highestScoreExpert = score;
                                        }
                                        if (ts2 < ts1)
                                        {
                                            bestCompleteTimeExpert = timeStr;
                                        }
                                        int minutes3 = Convert.ToInt32(timeStr.Split(':')[0]);
                                        int seconds3 = Convert.ToInt32(timeStr.Split(':')[1]);
                                        seconds3 += minutes3 * 60;
                                        midCountExpert++;
                                        midTimeExpert += seconds3;
                                        break;
                                }
                            }
                        }
                    }
                    switch (selectedDiff)
                    {
                        case difficulties.beginner:
                            if(bestCompleteTimeEasy!="59:59")
                            {
                                txtBestTimeShow.Text = bestCompleteTimeEasy;
                            }
                            if(highestScoreEasy>0)
                            {
                                txtBestScoreShow.Text = highestScoreEasy.ToString();
                            }
                            if(midCountEasy>0)
                            {
                                int esBuf = midTimeEasy / midCountEasy;
                                string res = "";
                                string res2 = "";
                                if((esBuf / 60) <10)
                                {
                                    res = "0" + esBuf / 60;
                                }
                                else
                                {
                                    res = (esBuf / 60).ToString();
                                }
                                if((esBuf % 60) < 10)
                                {
                                    res2 = "0" + esBuf % 60;
                                }
                                else
                                {
                                    res2 = (esBuf % 60).ToString();
                                }
                                txtMidTimeShow.Text = res + ":" + res2;
                            }
                            else
                            {
                                txtMidTimeShow.Text = "--:--";
                            }
                            break;
                        case difficulties.intermediate:
                            if (bestCompleteTimeMid != "59:59")
                            {
                                txtBestTimeShow.Text = bestCompleteTimeMid;
                            }
                            if (highestScoreMid > 0)
                            {
                                txtBestScoreShow.Text = highestScoreMid.ToString();
                            }
                            if (midCountMid > 0)
                            {
                                int midBuf = midTimeMid / midCountMid;
                                string res = "";
                                string res2 = "";
                                if ((midBuf / 60) < 10)
                                {
                                    res = "0" + midBuf / 60;
                                }
                                else
                                {
                                    res = (midBuf / 60).ToString();
                                }
                                if ((midBuf % 60) < 10)
                                {
                                    res2 = "0" + midBuf % 60;
                                }
                                else
                                {
                                    res2 = (midBuf % 60).ToString();
                                }
                                txtMidTimeShow.Text = res + ":" + res2;
                            }
                            else
                            {
                                txtMidTimeShow.Text = "--:--";
                            }
                            break;
                        case difficulties.expert:
                            if (bestCompleteTimeHard != "59:59")
                            {
                                txtBestTimeShow.Text = bestCompleteTimeHard;
                            }
                            if (highestScoreHard > 0)
                            {
                                txtBestScoreShow.Text = highestScoreHard.ToString();
                            }
                            if (midCountHard > 0)
                            {
                                int hdBuf = midTimeHard / midCountHard;
                                string res = "";
                                string res2 = "";
                                if ((hdBuf / 60) < 10)
                                {
                                    res = "0" + hdBuf / 60;
                                }
                                else
                                {
                                    res = (hdBuf / 60).ToString();
                                }
                                if ((hdBuf % 60) < 10)
                                {
                                    res2 = "0" + hdBuf % 60;
                                }
                                else
                                {
                                    res2 = (hdBuf % 60).ToString();
                                }
                                txtMidTimeShow.Text = res + ":" + res2;
                            }
                            else
                            {
                                txtMidTimeShow.Text = "--:--";
                            }
                            break;
                        case difficulties.professional:
                            if (bestCompleteTimeExpert != "59:59")
                            {
                                txtBestTimeShow.Text = bestCompleteTimeExpert;
                            }
                            if (highestScoreExpert > 0)
                            {
                                txtBestScoreShow.Text = highestScoreExpert.ToString();
                            }
                            if (midCountExpert > 0)
                            {
                                int exBuf = midTimeExpert / midCountExpert;
                                string res = "";
                                string res2 = "";
                                if ((exBuf / 60) < 10)
                                {
                                    res = "0" + exBuf / 60;
                                }
                                else
                                {
                                    res = (exBuf / 60).ToString();
                                }
                                if ((exBuf % 60) < 10)
                                {
                                    res2 = "0" + exBuf % 60;
                                }
                                else
                                {
                                    res2 = (exBuf % 60).ToString();
                                }
                                txtMidTimeShow.Text = res + ":" + res2;
                            }
                            else
                            {
                                txtMidTimeShow.Text = "--:--";
                            }
                            break;
                    }
                    txtGamesPlayedCount.Text = totalGamesPlayed.ToString();
                    txtGamesPlayedCountLegacy.Text = totalLegacyPlayed.ToString();
                    double resultWR = (double)totalGamesWins / (totalGamesPlayed-totalGamesDraws)*100;
                    if(totalGamesPlayed>0) txtWR.Text = Math.Round(resultWR,1).ToString()+"%";
                    if (!needToCountLevels)
                    {
                        lvlCount = Convert.ToInt32(cutFromNode(scoreStr, paramType.levelPlayer));
                        expCounter = Convert.ToInt32(cutFromNode(scoreStr, paramType.currentExp));
                    }
                    needToCountLevels = false;
                    txtLevel.Text = "Уровень игрока ["+lvlCount+"]\n("+expCounter+"/1000)";
                    dataList = dataList.OrderBy(x => x.sc).Reverse().ToList();
                    if(dataList.Count>10)dataList.RemoveRange(10, dataList.Count-10);
                    scoreGrid.ItemsSource = dataList; 
                    scoreGrid.Items.Refresh();
                    try
                    {
                        checkBonuses(cutFromNode(scoreStr, paramType.bonusCodes));
                        checkThemesUnlocked(cutFromNode(scoreStr,paramType.themesUnlocked));
                        checkAchievments(cutFromNode(scoreStr, paramType.achievments));
                    }
                    catch
                    {
                        checkBonuses("");
                        checkThemesUnlocked("");
                        checkAchievments("");
                    }
                    checkMaterials(cutFromNode(scoreStr, paramType.scrap_count),cutFromNode(scoreStr, paramType.powder_count), cutFromNode(scoreStr, paramType.dirt_count));
                    if(Settings.Default.ZakharPlayer)
                    {
                        zakhar_player = true;
                        worker_player = false;
                    }
                    if(Settings.Default.LehaPlayer)
                    {
                        leha_player = true;
                        worker_player = false;
                    }
                    if (Settings.Default.EvgenPlayer)
                    {
                        evgen_player = true;
                        worker_player = false;
                    }
                    if (Settings.Default.WorkerPlayer)
                    {
                        worker_player = true;
                    }

                    openCharacters = cutFromNode(scoreStr, paramType.openCharacters);
                    if(openCharacters.Contains("zakhar"))
                    {
                        zakhar_level = Convert.ToInt32(cutFromNode(scoreStr, paramType.zakhar_level));
                    }    
                    if(openCharacters.Contains("evgen"))
                    {
                        evgen_level = Convert.ToInt32(cutFromNode(scoreStr, paramType.evgen_level));
                    }
                    if(openCharacters.Contains("leha"))
                    {
                        leha_level = Convert.ToInt32(cutFromNode(scoreStr, paramType.leha_level));
                    }
                    checkOpenedThemes();
                    checkOpenedAchievments();
                    if (scoreStr.Contains("opraMinec"))
                    {
                        int compensation = Convert.ToInt32(cutFromNode(scoreStr,paramType.opraMinec));
                        shoppoints += compensation*100;
                        shoppointL.Content = shoppoints;
                        MSGBox.Show("Из-за изменений в принципе работы \"магазина\" вам были возвращено "+compensation*100+" очков!");
                        EncryptDecryptFile(filePath);
                        scoreBoardUpdate();
                    }
                    else
                    {
                        EncryptDecryptFile(filePath);
                        debugstring = File.ReadAllText(filePath);
                    }
                }
            }
        }

        private void checkMaterials(string scr, string pow, string dirt)
        {
            try
            {
                scrap_count = Convert.ToInt32(scr);
                powder_count = Convert.ToInt32(pow);
                flags_count = Convert.ToInt32(dirt);
            }
            catch
            {
                scrap_count = 0;
                powder_count = 0;
                flags_count = 0;
            }
            scrapTxt.Text = "Част.: " + scrap_count;
            powderTxt.Text = "Пыл.: " + powder_count;
            flagsTxt.Text = "Флаж.: " + flags_count;
        }

        private void checkBonuses(string inputString)
        {
            bonusCodesStr = inputString;
        }

        private void checkThemesUnlocked(string inputString)
        {
            themesUnlocked = inputString;
        }

        private void checkOpenedThemes()
        {
            if(themesUnlocked.Contains("goldenTheme"))
            {
                txtGoldenTheme.Text = "Золотая: ";
                BuyGoldenTheme.Visibility = Visibility.Collapsed;
                UseGoldenTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtGoldenTheme.Text = "Золотая (50000): ";
                BuyGoldenTheme.Visibility = Visibility.Visible;
                UseGoldenTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("depthsTheme"))
            {
                txtDepthsTheme.Text = "Глубины: ";
                BuyDepthsTheme.Visibility = Visibility.Collapsed;
                UseDepthsTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtDepthsTheme.Text = "Глубины (25000): ";
                BuyDepthsTheme.Visibility = Visibility.Visible;
                UseDepthsTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("skydiveTheme"))
            {
                txtSkydiveTheme.Text = "Небеса: ";
                BuySkydiveTheme.Visibility = Visibility.Collapsed;
                UseSkydiveTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtSkydiveTheme.Text = "Небеса (25000): ";
                BuySkydiveTheme.Visibility = Visibility.Visible;
                UseSkydiveTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("grassyTheme"))
            {
                txtGrassyTheme.Text = "Луга: ";
                BuyGrassyTheme.Visibility = Visibility.Collapsed;
                UseGrassyTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtGrassyTheme.Text = "Луга (25000): ";
                BuyGrassyTheme.Visibility = Visibility.Visible;
                UseGrassyTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("autumnTheme"))
            {
                txtAutumnTheme.Text = "Осень: ";
                BuyAutumnTheme.Visibility = Visibility.Collapsed;
                UseAutumnTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtAutumnTheme.Text = "Осень (25000): ";
                BuyAutumnTheme.Visibility = Visibility.Visible;
                UseAutumnTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("scifiTheme"))
            {
                txtScifiTheme.Text = "Sci-Fi: ";
                BuyScifiTheme.Visibility = Visibility.Collapsed;
                UseScifiTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtScifiTheme.Text = "Sci-Fi (25000): ";
                BuyScifiTheme.Visibility = Visibility.Visible;
                UseScifiTheme.Visibility = Visibility.Collapsed;
            }
            if (themesUnlocked.Contains("cyberpunkTheme"))
            {
                txtCyberpunkTheme.Text = "Киберпанк: ";
                BuyCyberpunkTheme.Visibility = Visibility.Collapsed;
                UseCyberpunkTheme.Visibility = Visibility.Visible;
            }
            else
            {
                txtCyberpunkTheme.Text = "Киберпанк (75000): ";
                BuyCyberpunkTheme.Visibility = Visibility.Visible;
                UseCyberpunkTheme.Visibility = Visibility.Collapsed;
            }
        }

        private void checkAchievments(string inputString)
        {
            achievmentsStr = inputString;
        }

        private async Task revealAllMines()
        {
            App.setGlowEffect(Settings.Default.Theme,false);
            await Task.Run(() =>
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        if (minefield[row, col] && !flagged[row,col])
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Image bomb = new Image();
                                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/bimba.png"));
                                bomb.Source = bitmap;
                                bomb.Width = 20;
                                bomb.Height = 20;
                                buttons[row, col].Content = bomb;
                                buttons[row, col].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonMineBackgroundBrush) } };
                            });
                        }
                        if (adjacentMines[row,col]>0)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                countNoMoreScore = true;
                                tryToReveal(row, col);
                            });
                        }
                    }
                }
            });           
        }

        static private string cutFromNode(string node, paramType pt)
        {
            string returnString = "";
            switch(pt)
            {
                case paramType.score:
                    if (node.Contains("SCORE"))
                    {
                        string pattern = @"SCORE=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.time:
                    if (node.Contains("TIME"))
                    {
                        string pattern = @"TIME=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.date:
                    if (node.Contains("DATE"))
                    {
                        string pattern = @"DATE=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.shoppoints:
                    if(node.Contains("shoppoint"))
                    {
                        string pattern = @"shoppoint=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.bonusCodes:
                    if (node.Contains("bonusCodes"))
                    {
                        string pattern = @"bonusCodes=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.themesUnlocked:
                    if (node.Contains("themesUnlocked"))
                    {
                        string pattern = @"themesUnlocked=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.achievments:
                    if (node.Contains("achievments"))
                    {
                        string pattern = @"achievments=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.opraMinec:
                    if (node.Contains("opraMinec"))
                    {
                        string pattern = @"opraMinec=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.levelPlayer:
                    if (node.Contains("levelPlayer"))
                    {
                        string pattern = @"levelPlayer=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.currentExp:
                    if (node.Contains("currentExp"))
                    {
                        string pattern = @"currentExp=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.result:
                    if (node.Contains("result"))
                    {
                        string pattern = @"result=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.difficulty:
                    if (node.Contains("difficulty"))
                    {
                        string pattern = @"difficulty=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.scrap_count:
                    if (node.Contains("scrap_count"))
                    {
                        string pattern = @"scrap_count=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.powder_count:
                    if (node.Contains("powder_count"))
                    {
                        string pattern = @"powder_count=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.dirt_count:
                    if (node.Contains("dirt_count"))
                    {
                        string pattern = @"dirt_count=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.openCharacters:
                    if (node.Contains("openCharacters"))
                    {
                        string pattern = @"openCharacters=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.zakhar_level:
                    if (node.Contains("zakhar_level"))
                    {
                        string pattern = @"zakhar_level=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.evgen_level:
                    if (node.Contains("evgen_level"))
                    {
                        string pattern = @"evgen_level=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
                case paramType.leha_level:
                    if (node.Contains("leha_level"))
                    {
                        string pattern = @"leha_level=""([^""]*)""";
                        Match match = Regex.Match(node, pattern);
                        if (match.Success)
                        {
                            returnString = match.Groups[1].Value;
                        }
                    }
                    break;
            }
            return returnString;
        }

        private void easy_Checked(object sender, RoutedEventArgs e)
        {
            mid.IsChecked = false;
            hard.IsChecked = false;
            custom.IsChecked = false;
            expert.IsChecked = false;
            txtBestScoreShow.Text = highestScoreEasy.ToString();
            if(bestCompleteTimeEasy!= "59:59")
            {
                txtBestTimeShow.Text = bestCompleteTimeEasy;
            }
            else
            {
                txtBestTimeShow.Text = "--:--";
            }
            if (midCountEasy > 0)
            {
                int ezBuf = midTimeEasy / midCountEasy;
                string res = "";
                string res2 = "";
                if ((ezBuf / 60) < 10)
                {
                    res = "0" + ezBuf / 60;
                }
                else
                {
                    res = (ezBuf / 60).ToString();
                }
                if ((ezBuf % 60) < 10)
                {
                    res2 = "0" + ezBuf % 60;
                }
                else
                {
                    res2 = (ezBuf % 60).ToString();
                }
                txtMidTimeShow.Text = res + ":" + res2;
            }
            else
            {
                txtMidTimeShow.Text = "--:--";
            }
            customPanel.Visibility = Visibility.Collapsed;
            selectedDiff = difficulties.beginner;
        }


        private void expert_Checked(object sender, RoutedEventArgs e)
        {
            easy.IsChecked = false;
            mid.IsChecked = false;
            hard.IsChecked = false;
            custom.IsChecked = false;
            txtBestScoreShow.Text = highestScoreExpert.ToString();
            if (bestCompleteTimeExpert != "59:59")
            {
                txtBestTimeShow.Text = bestCompleteTimeExpert;
            }
            else
            {
                txtBestTimeShow.Text = "--:--";
            }
            if (midCountExpert > 0)
            {
                int exBuf = midTimeExpert / midCountExpert;
                string res = "";
                string res2 = "";
                if ((exBuf / 60) < 10)
                {
                    res = "0" + exBuf / 60;
                }
                else
                {
                    res = (exBuf / 60).ToString();
                }
                if ((exBuf % 60) < 10)
                {
                    res2 = "0" + exBuf % 60;
                }
                else
                {
                    res2 = (exBuf % 60).ToString();
                }
                txtMidTimeShow.Text = res + ":" + res2;
            }
            else
            {
                txtMidTimeShow.Text = "--:--";
            }
            customPanel.Visibility = Visibility.Collapsed;
            selectedDiff = difficulties.professional;
        }

        private void mid_Checked(object sender, RoutedEventArgs e)
        {
            expert.IsChecked = false;
            easy.IsChecked = false;
            hard.IsChecked = false;
            custom.IsChecked = false;
            txtBestScoreShow.Text = highestScoreMid.ToString();
            if (bestCompleteTimeMid != "59:59")
            {
                txtBestTimeShow.Text = bestCompleteTimeMid;
            }
            else
            {
                txtBestTimeShow.Text = "--:--";
            }
            if (midCountMid > 0)
            {
                int midBuf = midTimeMid / midCountMid;
                string res = "";
                string res2 = "";
                if ((midBuf / 60) < 10)
                {
                    res = "0" + midBuf / 60;
                }
                else
                {
                    res = (midBuf / 60).ToString();
                }
                if ((midBuf % 60) < 10)
                {
                    res2 = "0" + midBuf % 60;
                }
                else
                {
                    res2 = (midBuf % 60).ToString();
                }
                txtMidTimeShow.Text = res + ":" + res2;
            }
            else
            {
                txtMidTimeShow.Text = "--:--";
            }
            customPanel.Visibility = Visibility.Collapsed;
            selectedDiff = difficulties.intermediate;
        }

        private void hard_Checked(object sender, RoutedEventArgs e)
        {
            expert.IsChecked = false;
            easy.IsChecked = false;
            mid.IsChecked= false;
            custom.IsChecked= false;
            txtBestScoreShow.Text = highestScoreHard.ToString();
            if (bestCompleteTimeHard != "59:59")
            {
                txtBestTimeShow.Text = bestCompleteTimeHard;
            }
            else
            {
                txtBestTimeShow.Text = "--:--";
            }
            if (midCountHard > 0)
            {
                int hdBuf = midTimeHard / midCountHard;
                string res = "";
                string res2 = "";
                if ((hdBuf / 60) < 10)
                {
                    res = "0" + hdBuf / 60;
                }
                else
                {
                    res = (hdBuf / 60).ToString();
                }
                if ((hdBuf % 60) < 10)
                {
                    res2 = "0" + hdBuf % 60;
                }
                else
                {
                    res2 = (hdBuf % 60).ToString();
                }
                txtMidTimeShow.Text = res + ":" + res2;
            }
            else
            {
                txtMidTimeShow.Text = "--:--";
            }
            customPanel.Visibility = Visibility.Collapsed;
            selectedDiff = difficulties.expert;
        }

        private void custom_Checked(object sender, RoutedEventArgs e)
        {
            expert.IsChecked = false;
            easy.IsChecked = false;
            mid.IsChecked = false;
            hard.IsChecked = false;
            txtBestTimeShow.Text = "--:--";
            txtMidTimeShow.Text = "--:--";
            txtBestScoreShow.Text = "----";
            customPanel.Visibility = Visibility.Visible;
            selectedDiff = difficulties.custom;
        }

        private void rowsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (colBox.Text != "")
            {
                recMine.Content = "рекоменд.: " + (Convert.ToInt32(rowsBox.Text) * Convert.ToInt32(colBox.Text) * 0.16).ToString();
            }
        }

        private void colBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rowsBox.Text != "")
            {
                recMine.Content = "рекоменд.:" + (Convert.ToInt32(rowsBox.Text) * Convert.ToInt32(colBox.Text) * 0.16).ToString();
            }
        }

        private void patchNotesBtn_Click(object sender, RoutedEventArgs e)
        {
            patchNotes wn = new patchNotes();
            wn.Show();
            wn.InvalidateVisual();
            wn.UpdateLayout();
        }

        private void rowsBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(rowsBox.Text) > 50) rowsBox.Text = "50";
                if (Convert.ToInt32(rowsBox.Text) < 5) rowsBox.Text = "5";
            }
            catch
            {
                rowsBox.Text = "10";
            }

        }

        private void colBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(colBox.Text) > 50) colBox.Text = "50";
                if (Convert.ToInt32(colBox.Text) < 5) colBox.Text = "5";
            }
            catch
            {
                colBox.Text="10";
            }

        }

        private void minesBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (minesBox.Text!="")
                {
                    if (Convert.ToInt32(minesBox.Text) <= 1) minesBox.Text = "1";
                }
                else
                {
                    if (Convert.ToInt32(rowsBox.Text)>0&& Convert.ToInt32(colBox.Text) > 0)
                    {
                        minesBox.Text = (Convert.ToInt32(rowsBox.Text) * Convert.ToInt32(colBox.Text) * 0.16).ToString();
                    }
                }
            }
            catch
            {
                minesBox.Text = "";
            }
        }

        private void shop_Click(object sender, RoutedEventArgs e)
        {
            if(shopPanel.Visibility==Visibility.Collapsed)
            {
                this.Width += 250;
                hideMenu.Margin = new Thickness(0, 0, 450, 0);
                characterChoose.Margin = new Thickness(0, 25, 450, 0);
                mainPanel.Margin = new Thickness(0, 0, 250, 0);
                shopPanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.Width -= 250;
                hideMenu.Margin = new Thickness(0, 0, 200, 0);
                characterChoose.Margin = new Thickness(0, 25, 200, 0);
                mainPanel.Margin = new Thickness(0, 0, 0, 0);
                shopPanel.Visibility = Visibility.Collapsed;
            }    

        }

        private void openRandomMineLabel_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver) return;
            if (shoppoints>openRandomMinePrice)
            {
                if (openedMineCount == mineCount)
                {
                    return;
                }
                shoppoints -= openRandomMinePrice;
                shoppointL.Content = shoppoints;
                findRandomMine();
            }
        }
        private void findRandomMine()
        {
            int col = rnd.Next(columns);
            int row = rnd.Next(rows);
            if (minefield[row, col] && !flagged[row,col] && !shopRevealed[row,col])
            {
                buttons[row, col].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BorderBrushProperty, (Brush)buttonMineBackgroundBrush), new Setter(Button.BorderThicknessProperty, new Thickness(3)) } };
                shopRevealed[row, col] = true;
                openedMineCount++;
                if(openedMineCount==mineCount)
                {
                    return;
                }
            }
            else
            {
                findRandomMine();
            }
        }
        private void openRandomSafeLabel_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver) return;
            if (shoppoints >= openRandomTilePrice)
            {
                if (openedTileCount == (rows * columns) - mineCount) return;
                shoppoints -= openRandomTilePrice;
                shoppointL.Content = shoppoints;
                findRandomTile();
            }
        }
        private void findRandomTile()
        {
            int col = rnd.Next(columns);
            int row = rnd.Next(rows);
            if (!minefield[row, col] && !revealed[row, col])
            {
                tryToReveal(row, col);
            }
            else
            {
                findRandomTile();
            }
        }

        private static void EncryptDecryptFile(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            for(int i =0; i<fileBytes.Length;i++)
            {
                fileBytes[i] = (byte)(fileBytes[i] ^ Key[i % Key.Length]);
            }
            File.WriteAllBytes(filePath, fileBytes);
        }

        private void activateCode_Click(object sender, RoutedEventArgs e)
        {
            string checkCode = bonusTextBox.Text;
            if (checkCode == "") return;
            bonusCodeEnter = true;
            //adminCode
            if(checkCode.Contains("IamAtomic"))
            {
                shoppoints += 40000;
                scrap_count += 1000;
                powder_count += 1000;
                flags_count += 1000;
                shoppointL.Content = shoppoints;
                bonusTextBox.Text = "";
                scoreBoardUpdate();
            }
            //all achievments
            if (checkCode.Contains("OldGold"))
            {
                if (!bonusCodesStr.Contains("OldGold"))
                {
                    checkAchievments(achievments.hopeOfHumanity);
                    checkAchievments(achievments.speedster);
                    checkAchievments(achievments.worthy);
                    bonusCodesStr += ".OldGold.";
                    bonusTextBox.Text = "";
                }
                else
                {
                    bonusTextBox.Text = "";
                    MSGBox.Show("Данный бонус-код уже активирован!");
                }
            }
            //code for 14.02.2025 EXPIRED
            if (checkCode.Contains("Bonus140225"))
            {
                //if (!bonusCodesStr.Contains("Bonus140225"))
                //{
                //    shoppoints += 1400;
                //    shoppointL.Content = shoppoints;
                //    bonusCodesStr += ".Bonus140225.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 1400 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for 23.02.2025 EXPIRED
            if (checkCode.Contains("Bonus230225"))
            {
                //if (!bonusCodesStr.Contains("Bonus230225"))
                //{
                //    shoppoints += 2300;
                //    bonusCodesStr += ".Bonus230225.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 2300 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch EXPIRED
            if (checkCode.Contains("BonusThemes"))
            {
                //if (!bonusCodesStr.Contains("BonusThemes"))
                //{
                //    shoppoints += 2500;
                //    bonusCodesStr += ".BonusThemes.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 2500 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch EXPIRED
            if (checkCode.Contains("BonusImages"))
            {
                //if (!bonusCodesStr.Contains("BonusImages"))
                //{
                //    shoppoints += 5000;
                //    bonusCodesStr += ".BonusImages.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 5000 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for 01.03.25 EXPIRED
            if (checkCode.Contains("ВеснаПришла"))
            {
                //if (!bonusCodesStr.Contains("ВеснаПришла"))
                //{
                //    shoppoints += 1000;
                //    bonusCodesStr += ".ВеснаПришла.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 1000 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch
            if(checkCode.Contains("BonusNeonGlow"))
            {
                //if(!bonusCodesStr.Contains("BonusNeonGlow"))
                //{
                //    shoppoints += 7000;
                //    bonusCodesStr += ".bonusNeonGlow.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 7000 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch
            if (checkCode.Contains("BonusStats"))
            {
                //if (!bonusCodesStr.Contains("bonusStats"))
                //{
                //    shoppoints += 3500;
                //    bonusCodesStr += ".bonusStats.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 3500 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch
            if (checkCode.Contains("BonusAchiev"))
            {
                //if (!bonusCodesStr.Contains("bonusAchiev"))
                //{
                //    shoppoints += 5000;
                //    bonusCodesStr += ".bonusAchiev.";
                //    bonusTextBox.Text = "";
                //    scoreBoardUpdate();
                //    MSGBox.Show("Активация успешна! Зачислено 5000 поинтов магазина!");
                //}
                //else
                //{
                //    bonusTextBox.Text = "";
                //    MSGBox.Show("Данный бонус-код уже активирован!");
                //}
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for big patch
            if (checkCode.Contains("BonusSpeedRun"))
            {
                /*if (!bonusCodesStr.Contains("bonusSpeedRun"))
                {
                    shoppoints += 4500;
                    bonusCodesStr += ".bonusSpeedRun.";
                    bonusTextBox.Text = "";
                    scoreBoardUpdate();
                    MSGBox.Show("Активация успешна! Зачислено 4500 поинтов магазина!");
                }
                else
                {
                    bonusTextBox.Text = "";
                    MSGBox.Show("Данный бонус-код уже активирован!");
                }*/
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for filler patch
            if (checkCode.Contains("BonusFiller"))
            {
                /*if (!bonusCodesStr.Contains("bonusFiller"))
                {
                    shoppoints += 1500;
                    bonusCodesStr += ".bonusFiller.";
                    bonusTextBox.Text = "";
                    scoreBoardUpdate();
                    MSGBox.Show("Активация успешна! Зачислено 1500 поинтов магазина!");
                }
                else
                {
                    bonusTextBox.Text = "";
                    MSGBox.Show("Данный бонус-код уже активирован!");
                }*/
                MSGBox.Show("Срок действия бонус-кода истек!");
            }
            //code for release? patch
            if (checkCode.Contains("WeCameHereToConquer"))
            {
                if (!bonusCodesStr.Contains("WeCameHereToConquer"))
                {
                    shoppoints += 7500;
                    bonusCodesStr += ".WeCameHereToConquer.";
                    bonusTextBox.Text = "";
                    scoreBoardUpdate();
                    MSGBox.Show("Активация успешна! Зачислено 7500 поинтов магазина!");
                }
                else
                {
                    bonusTextBox.Text = "";
                    MSGBox.Show("Данный бонус-код уже активирован!");
                }
            }
            //first code for materials
            if (checkCode.Contains("SomeMaterials")&&achievmentsStr.Contains("worthy"))
            {
                if (!bonusCodesStr.Contains("SomeMaterials"))
                {
                    scrap_count += 20;
                    powder_count += 40;
                    flags_count += 2;
                    bonusCodesStr += ".SomeMaterials.";
                    bonusTextBox.Text = "";
                    scoreBoardUpdate();
                    MSGBox.Show("Активация успешна! Зачислено 20 \"Частей\", 40 \"Пыли\" и 2 \"Флажка\"");
                }
                else
                {
                    bonusTextBox.Text = "";
                    MSGBox.Show("Данный бонус-код уже активирован!");
                }
            }
            //secret code
            if (checkCode.Contains("киберпсих"))
            {
                if(achievmentsStr.Contains("hopeOfHumanity"))
                {
                    if (!bonusCodesStr.Contains("киберпсих"))
                    {
                        shoppoints += 20770;
                        bonusCodesStr += ".киберпсих.";
                        bonusTextBox.Text = "";
                        scoreBoardUpdate();
                        MSGBox.Show("Ахах=ахахах-а-ах ==))");
                        MSGBox.Show("Активация активирована! Активировано 2̸̡̬͙̺̍̆͑̈́0̸̠̯̏7̶̯̗̘͇̼͌̈́̽7̸̺̬̥̽͗̎̿0̸͚̣͎̏ активаций активации!");
                        MSGBox.Show("Активация дезактивация активированоооооо ха-ахаа-хах=хахх");
                        MSGBox.Show("ХААААААААААААААААААААААААААААААААААААААААХАХАХАХАХА");
                        MSGBox.Show("Успешно активированоаоваоова");
                        MSGBox.Show("Бонус-код успеепсу док-суноБ");
                        MSGBox.Show("00011101011 10100111 101010011 1111000");
                        MSGBox.Show("Блин, вот это даа...");
                        MSGBox.Show("Активация успешна! Зачислено 1 поинтов магазина!");
                        MSGBox.Show("Активация успешна! Зачислено (ну ладно, не 0) поинтов магазина!");
                        MSGBox.Show("Активация успешна! Зачислено 0 поинтов магазина!");
                    }
                    else
                    {
                        bonusTextBox.Text = "";
                        MSGBox.Show("Ох черт...");
                        MSGBox.Show("Хаахахахаххахаха круто))))");
                        MSGBox.Show("Данный бонус-код уже бонус-код!");
                        MSGBox.Show("Данный бонус-код уже нененене активирован!");
                        MSGBox.Show("Данный бонус-код уже сново активирован!");
                        MSGBox.Show("Данный бонус-код уже не совсем активирован!");
                        MSGBox.Show("Данный бонус-код уже уже активирован!");
                        MSGBox.Show("Данный бонус-код уже не активирован!");
                        MSGBox.Show("Данный бонус-код уже активирован!");
                    }
                }
            }
        }

        private void changeTheme()
        {
            if(Settings.Default.Theme=="DarkTheme")
            {
                ThemeToggleButton.IsChecked= true;
            }
            if (Settings.Default.Theme == "LightThemeAlternate")
            {
                CursorToggleButton.IsChecked = true;
            }
            if(Settings.Default.Theme=="GoldenTheme")
            {
                if(!themesUnlocked.Contains("goldenTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "DepthsTheme")
            {
                if (!themesUnlocked.Contains("depthsTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "SkydiveTheme")
            {
                if (!themesUnlocked.Contains("skydiveTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "CyberpunkTheme")
            {
                if (!themesUnlocked.Contains("cyberpunkTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "GrassyTheme")
            {
                if (!themesUnlocked.Contains("grassyTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "AutumnTheme")
            {
                if (!themesUnlocked.Contains("autumnTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            if (Settings.Default.Theme == "ScifiTheme")
            {
                if (!themesUnlocked.Contains("scifiTheme"))
                {
                    Settings.Default.Theme = "LightTheme";
                }
            }
            buttonBackgroundBrush = Application.Current.TryFindResource("ButtonBackgroundBrush");
            buttonFlaggedBackgroundBrush = Application.Current.TryFindResource("ButtonFlaggedBackgroundBrush");
            buttonRevealedBackgroundBrush = Application.Current.TryFindResource("ButtonRevealedBackgroundBrush");
            buttonMineBackgroundBrush = Application.Current.TryFindResource("ButtonMineBackgroundBrush");
            buttonAdjacentBackgroundBrush = Application.Current.TryFindResource("ButtonAdjacentBackgroundBrush");
            buttonSonarBackgroundBrush = Application.Current.TryFindResource("ButtonSonarBackgroundBrush");
            buttonForegroundBrush = Application.Current.TryFindResource("ButtonForegroundBrush");
            labelBackgroundBrush = Application.Current.TryFindResource("BackgroundBrush");
            labelForegroundBrush = Application.Current.TryFindResource("ForegroundBrush");
            labelWinBackgroundBrush = Application.Current.TryFindResource("ScoreLabelWinBackgroundBrush");
            labelWinForegroundBrush = Application.Current.TryFindResource("ScoreLabelWinForegroundBrush");
            labelLoseBackgroundBrush = App.Current.TryFindResource("ScoreLabelLoseBackgoundBrush");
            labelLoseForegroundBrush = App.Current.TryFindResource("ScoreLabelLoseForegroundBrush");
            buttonStyle = (Style)FindResource("BaseButtonStyle");
            buttonBoostHoverBackground = App.Current.TryFindResource("ButtonHoverBoostActiveBackgroundBrush");
            labelStyle = (Style)FindResource("LabelStyle");
            textblockStyle = (Style)FindResource("TextBlockStyle");
            textboxStyle = (Style)FindResource("TextBoxStyle");
            txtBoxHeader = (Style)FindResource("MyHeaderTextStyle");
            checkboxStyle = (Style)FindResource("CheckBoxStyle");
            windowStyle = (Style)FindResource("MyWindowStyle");
            transparentBackground = App.Current.TryFindResource("TransparentBackgroundBrush");
            buttonGlowStyle = (Style)FindResource("GlowButtonStyle");
            //datagridStyle = (Style)FindResource("DataGridStyle");
            //if (Settings.Default.Theme == "CyberpunkTheme")
            //{
            //    fontSize = 8;
            //    startFontSize = 20;
            //    boostFontSize = 16;
            //    boost2FontSize = 13;
            //}
            //else
            //{
                fontSize = 14;
                startFontSize = 30;
                boostFontSize = 20;
                boost2FontSize = 16;
            // }
            if (Settings.Default.Theme == "CyberpunkTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/cback.jpg", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else if(Settings.Default.Theme == "ScifiTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/scifi.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else if(Settings.Default.Theme == "SkydiveTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/skydive.jpg", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else if(Settings.Default.Theme == "GrassyTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/grassy.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else if(Settings.Default.Theme == "DepthsTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/depths.jpeg", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else if(Settings.Default.Theme == "AutumnTheme")
            {
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/autumn.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill
                };
                this.Background = imageBrush;
                labelBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                shopPanel.Background = (Brush)transparentBackground;
                mainPanel.Background = (Brush)transparentBackground;
            }
            else
            {
                this.Background = (Brush)labelBackgroundBrush;
                shopPanel.Background = (Brush)labelBackgroundBrush;
                mainPanel.Background = (Brush)labelBackgroundBrush;
            }
            paintLabels();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            scoreBoardUpdate();
        }

        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == System.Windows.WindowState.Normal)
                {
                    WindowState = System.Windows.WindowState.Maximized;
                    TitleGrid.Margin = new Thickness(0, 6, 0, 0);
                }
                else
                {
                    WindowState = System.Windows.WindowState.Normal;
                    TitleGrid.Margin = new Thickness(0);
                }
            }
            else
            {
                if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
                    return;
                this.DragMove();
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Normal)
            {
                WindowState = System.Windows.WindowState.Maximized;
                TitleGrid.Margin = new Thickness(0, 6, 0, 0);
            }
            else
            {
                WindowState = System.Windows.WindowState.Normal;
                TitleGrid.Margin = new Thickness(0);
            }
        }

        private void bMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if(depObj !=null)
            {
                for(int i=0; i< VisualTreeHelper.GetChildrenCount(depObj);i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj,i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach(T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return (T)childOfChild;
                    }
                }
            }
        }

        public void ThemeButtonFromWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                foreach (Button button in FindVisualChildren<Button>(window))
                {
                    if(button.Tag!=null)
                    {
                        Tuple<int, int> position = (Tuple<int, int>)button.Tag;
                        int row = position.Item1;
                        int col = position.Item2;
                        if (revealed[row, col])
                        {
                            button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
                            if (adjacentMines[row, col] > 0)
                            {

                                    button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonAdjacentBackgroundBrush) } };
                            }
                        }
                        else if (flagged[row,col])
                        {
                            Image bomb = new Image();
                            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                            bomb.Source = bitmap;
                            bomb.Width = 20;
                            bomb.Height = 20;
                            button.Content = bomb;

                                button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonFlaggedBackgroundBrush) } };
                        }
                        else if (gameOver && minefield[row,col])
                        {
                            Image bomb = new Image();
                            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/bimba.png"));
                            bomb.Source = bitmap;
                            bomb.Width = 20;
                            bomb.Height = 20;
                            button.Content = bomb;

                                button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonMineBackgroundBrush) } };
                        }
                        else
                        {
                            if(ThemeManager.CurrentTheme == "CyberpunkTheme"&& cbGlow)
                            {
                                button.Style = (Style)buttonGlowStyle;
                            }
                            else button.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
                        }
                    }
                    else
                    {
                        openRandomMineLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };

                        openRandomSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };

                        openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonRevealedBackgroundBrush) } };
                    }
                }
            }
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "LightTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            cursorStackPanel.Visibility = Visibility.Visible;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "DarkTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void BuyGoldenTheme_Click(object sender, RoutedEventArgs e)
        {
            if(shoppoints>=50000)
            {
                shoppoints -= 50000;
                shoppointL.Content = shoppoints;
                BuyGoldenTheme.Visibility = Visibility.Collapsed;
                UseGoldenTheme.Visibility = Visibility.Visible;
                txtGoldenTheme.Text = "Золотая: ";
                themesUnlocked += ".goldenTheme.";
            }
        }

        private void UseGoldenTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "GoldenTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void BuyDepthsTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 25000)
            {
                shoppoints -= 25000;
                shoppointL.Content = shoppoints;
                BuyDepthsTheme.Visibility = Visibility.Collapsed;
                UseDepthsTheme.Visibility = Visibility.Visible;
                txtDepthsTheme.Text = "Глубины: ";
                themesUnlocked += ".depthsTheme.";
            }
        }

        private void UseDepthsTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "DepthsTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void BuySkydiveTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 25000)
            {
                shoppoints -= 25000;
                shoppointL.Content = shoppoints;
                BuySkydiveTheme.Visibility = Visibility.Collapsed;
                UseSkydiveTheme.Visibility = Visibility.Visible;
                txtSkydiveTheme.Text = "Небеса: ";
                themesUnlocked += ".skydiveTheme.";
            }
        }

        private void UseSkydiveTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "SkydiveTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }


        private void BuyGrassyTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 25000)
            {
                shoppoints -= 25000;
                shoppointL.Content = shoppoints;
                BuyGrassyTheme.Visibility = Visibility.Collapsed;
                UseGrassyTheme.Visibility = Visibility.Visible;
                txtGrassyTheme.Text = "Луга: ";
                themesUnlocked += ".grassyTheme.";
            }
        }

        private void UseGrassyTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "GrassyTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }


        private void BuyAutumnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 25000)
            {
                shoppoints -= 25000;
                shoppointL.Content = shoppoints;
                BuyAutumnTheme.Visibility = Visibility.Collapsed;
                UseAutumnTheme.Visibility = Visibility.Visible;
                txtAutumnTheme.Text = "Осень: ";
                themesUnlocked += ".autumnTheme.";
            }
        }

        private void UseAutumnTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "AutumnTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }


        private void BuyScifiTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 25000)
            {
                shoppoints -= 25000;
                shoppointL.Content = shoppoints;
                BuyScifiTheme.Visibility = Visibility.Collapsed;
                UseScifiTheme.Visibility = Visibility.Visible;
                txtScifiTheme.Text = "Sci-Fi: ";
                themesUnlocked += ".scifiTheme.";
            }
        }

        private void UseScifiTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CurrentTheme = "ScifiTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void BuyCyberpunkTheme_Click(object sender, RoutedEventArgs e)
        {
            if (shoppoints >= 75000)
            {
                shoppoints -= 75000;
                shoppointL.Content = shoppoints;
                BuyCyberpunkTheme.Visibility = Visibility.Collapsed;
                UseCyberpunkTheme.Visibility = Visibility.Visible;
                txtCyberpunkTheme.Text = "Киберпанк: ";
                themesUnlocked += ".cyberpunkTheme.";
            }
        }

        private void UseCyberpunkTheme_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.CurrentTheme == "CyberpunkTheme") cbGlow = !cbGlow;
            else cbGlow = true;
            ThemeManager.CurrentTheme = "CyberpunkTheme";
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
            CursorToggleButton.IsChecked = false;
            cursorStackPanel.Visibility = Visibility.Hidden;
            changeTheme();
            ThemeButtonFromWindows();
        }

        private void CursorToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.Theme == "LightTheme")
            {
                ThemeManager.CurrentTheme = "LightThemeAlternate";
                Settings.Default.Theme = ThemeManager.CurrentTheme;
                Settings.Default.Save();
                changeTheme();
                ThemeButtonFromWindows();
            }
            else
            {
                CursorToggleButton.IsChecked = false;
                return;
            }
        }

        private void CursorToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.Theme == "LightThemeAlternate")
            {
                ThemeManager.CurrentTheme = "LightTheme";
                Settings.Default.Theme = ThemeManager.CurrentTheme;
                Settings.Default.Save();
                changeTheme();
                ThemeButtonFromWindows();
            }
        }

        private void openCloseSafeLabel_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver) return;
            if (scanBoostActive)
            {
                scanBoostActive = false;
                openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush), new Setter(Button.ForegroundProperty, (Brush)buttonForegroundBrush) } };
                return;
            }
            if(shoppoints>=150)
            {
                scanBoostActive = true;
                openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBoostHoverBackground), new Setter(Button.ForegroundProperty, (Brush)Brushes.Black) } };
            }
        }

        private void openCloseSafeTask()
        {
            int countTiles = 0;
            scanBoostActive = false;

            int moveBoost = 1;
            if(evgen_player&&evgen_level==5)
            {
                moveBoost = 2;
            }
            openCloseSafeLabel.Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonBackgroundBrush) } };
            if (adjacentMines[boostX, boostY] > 0 && revealed[boostX, boostY])
            {
                for (int i = boostX - moveBoost; i <= boostX + moveBoost; i++)
                {
                    for (int j = boostY - moveBoost; j <= boostY + moveBoost; j++)
                    {
                        if (i >= 0 && i < rows && j >= 0 && j < columns)
                        {
                            if ((i != boostX || j != boostY))
                            {
                                if (revealed[i, j]) countTiles++;
                                if (flagged[i, j]==true && minefield[i,j] == true) countTiles++;
                                if (minefield[i, j] == true && flagged[i,j]==false)
                                {
                                    minesLeft.Content = (Convert.ToInt32(minesLeft.Content) - 1).ToString();
                                    Image flag = new Image();
                                    BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                                    flag.Source = bitmap;
                                    flag.Width = 20;
                                    flag.Height = 20;
                                    buttons[i, j].Content = flag;
                                    buttons[i, j].Style = new Style(typeof(Button), buttonStyle) { Setters = { new Setter(Button.BackgroundProperty, (Brush)buttonFlaggedBackgroundBrush) } };
                                    flagged[i, j] = true; if (!shopRevealed[i, j]) openedMineCount++;
                                }
                                else if (!revealed[i,j])
                                {
                                    tryToReveal(i, j);
                                }

                            }
                        }
                        else countTiles++;
                    }
                }
            }
            else
            {
                return;
            }
            if (countTiles == 8) return;
            shoppoints -= openCloseSafePrice;
            shoppointL.Content = shoppoints;
        }

        private void easy_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                e.Handled = true;
            }
        }

        private void mid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                e.Handled = true;
            }
        }

        private void hard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                e.Handled = true;
            }
        }

        private void custom_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                e.Handled = true;
            }
        }

        private void expert_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(cbGlow)App.setGlowEffect(Settings.Default.Theme);
            ThemeButtonFromWindows();
            //paintLabels();
            scoreBoardLoad();
            changeTheme();
            if(Settings.Default.Theme=="LightTheme"|| Settings.Default.Theme == "LightThemeAlternate")
            {
                cursorStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                cursorStackPanel.Visibility = Visibility.Hidden;
            }
            if(Settings.Default.SpeedRun)
            {
                speedGameToggleButton.IsChecked = true;
                speedBoost = true;
            }
            else
            {
                speedGameToggleButton.IsChecked = false;
                speedBoost = false;
            }
            if(Settings.Default.ZakharPlayer)
            {
                zakhar_player = true;
            }
        }

        private void ScoresToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            LegacyScores.Visibility = Visibility.Visible;
            newScores.Visibility = Visibility.Collapsed;
        }

        private void ScoresToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            LegacyScores.Visibility = Visibility.Collapsed;
            newScores.Visibility = Visibility.Visible;
        }

        private void txtAchiveBtn_Click(object sender, RoutedEventArgs e)
        {
            Achievements aw = new Achievements(achievmentsStr);
            aw.ShowDialog();
        }

        private void speedGameToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            speedBoost = true;
            Settings.Default.SpeedRun = true;
            Settings.Default.Save();
        }

        private void speedGameToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            speedBoost = false;
            Settings.Default.SpeedRun = false;
            Settings.Default.Save();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch(selectedDiff)
            {
                case difficulties.beginner:
                    if(this.Width<(9*32+220))
                    {
                        mainPanel.Visibility = Visibility.Collapsed;
                        hideMenu.Content = "<";
                        hideMenu.Margin = new Thickness(0, 0, 0, 0);
                        characterChoose.Margin = new Thickness(0, 25, 0, 0);
                        if (shopPanel.Visibility == Visibility.Visible)
                        {
                            mainPanel.Margin = new Thickness(0, 0, 0, 0);
                            this.Width -= 250;
                            shopPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    break;
                case difficulties.intermediate:
                    if (this.Width < (16 * 32 + 220))
                    {
                        mainPanel.Visibility = Visibility.Collapsed;
                        hideMenu.Content = "<";
                        hideMenu.Margin = new Thickness(0, 0, 0, 0);
                        characterChoose.Margin = new Thickness(0, 25, 0, 0);
                        if (shopPanel.Visibility == Visibility.Visible)
                        {
                            mainPanel.Margin = new Thickness(0, 0, 0, 0);
                            this.Width -= 250;
                            shopPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    break;
                case difficulties.expert:
                    if (this.Width < (30 * 32 + 220))
                    {
                        mainPanel.Visibility = Visibility.Collapsed;
                        hideMenu.Content = "<";
                        hideMenu.Margin = new Thickness(0, 0, 0, 0);
                        characterChoose.Margin = new Thickness(0, 25, 0, 0);
                        if (shopPanel.Visibility == Visibility.Visible)
                        {
                            mainPanel.Margin = new Thickness(0, 0, 0, 0);
                            this.Width -= 250;
                            shopPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    break;
                case difficulties.professional:
                    if (this.Width < (30 * 32 + 220))
                    {
                        mainPanel.Visibility = Visibility.Collapsed;
                        hideMenu.Content = "<";
                        hideMenu.Margin = new Thickness(0, 0, 0, 0);
                        characterChoose.Margin = new Thickness(0, 25, 0, 0);
                        if (shopPanel.Visibility == Visibility.Visible)
                        {
                            mainPanel.Margin = new Thickness(0, 0, 0, 0);
                            this.Width -= 250;
                            shopPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    break;
            }
        }

        private void hideMenu_Click(object sender, RoutedEventArgs e)
        {
            if (mainPanel.Visibility == Visibility.Visible)
            {
                mainPanel.Visibility = Visibility.Collapsed;
                hideMenu.Content = "<";
                hideMenu.Margin = new Thickness(0, 0, 0, 0);
                characterChoose.Margin = new Thickness(0,25,0,0);
                if(shopPanel.Visibility == Visibility.Visible)
                {
                    mainPanel.Margin = new Thickness(0, 0, 0, 0);
                    this.Width -= 250;
                    shopPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                mainPanel.Visibility = Visibility.Visible;
                hideMenu.Content = ">";
                hideMenu.Margin = new Thickness(0, 0, 200, 0);
                characterChoose.Margin = new Thickness(0, 25, 200, 0);
            }
        }

        private void characterChoose_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect { Radius = 10 };
            this.Effect = blurEffect;
            CharacterSelection cs = new CharacterSelection(openCharacters, scrap_count, powder_count, flags_count);
            cs.ShowDialog();
            if (cs.DialogResult == true)
            {
                openCharacters = cs.returnCharacters;
                scrap_count = cs.returnScrap;
                powder_count = cs.returnPowder;
                flags_count = cs.returnFlags;
                this.Effect = null;
                scoreBoardUpdate();
            }
        }

        private void dayLoginCheck()
        {
            DateTime date1 = Settings.Default.LastLoginDate.Date;
            DateTime date2 = DateTime.Now.Date;
            loginStreak = Settings.Default.LoginStreak;
            if(date1!=date2)
            {
                if (DateComparator.IsOneDayAhaedIgnoringTime(date2, date1))
                {
                    if (loginStreak < 5) loginStreak++;
                    Settings.Default.LoginStreak = loginStreak;
                    Settings.Default.LastLoginDate = date2.Date;
                    Settings.Default.Save();
                }
                else
                {
                    Settings.Default.LastLoginDate = date2.Date;
                    Settings.Default.LoginStreak = 1;
                    loginStreak = 1;
                    Settings.Default.Save();
                }
                if (loginStreak > 0)
                {
                    switch (loginStreak)
                    {
                        case 1:
                            scrap_count += 5;
                            powder_count += 10;
                            scoreBoardUpdate();
                            MSGBox.Show("Ежедневный вход, день 1! Зачислено 5 \"Частей\", 10 \"Пыли\"");
                            break;
                        case 2:
                            scrap_count += 8;
                            powder_count += 13;
                            scoreBoardUpdate();
                            MSGBox.Show("Ежедневный вход, день 2! Зачислено 8 \"Частей\", 13 \"Пыли\"");
                            break;
                        case 3:
                            scrap_count += 25;
                            powder_count += 35;
                            scoreBoardUpdate();
                            MSGBox.Show("Ежедневный вход, день 3! Зачислено 25 \"Частей\", 35 \"Пыли\"");
                            break;
                        case 4:
                            scrap_count += 19;
                            powder_count += 34;
                            scoreBoardUpdate();
                            MSGBox.Show("Ежедневный вход, день 4! Зачислено 19 \"Частей\", 34 \"Пыли\"");
                            break;
                        case 5:
                            scrap_count += 22;
                            powder_count += 37;
                            flags_count += 1;
                            scoreBoardUpdate();
                            MSGBox.Show("Ежедневный вход, день 5! Зачислено 22 \"Частей\", 37 \"Пыли\", 1\"Флажок\"");
                            break;
                    }
                }
            }          
        }
    }
}