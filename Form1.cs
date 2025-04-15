using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Лабораторная_1_компиляторы
{
    public partial class Form1 : Form
    {
        private string currentFilePath = string.Empty; 
        private bool isTextChanged = false;
        //private Panel lineNumberPanel; // Панель для отображения номеров строк
        public Form1()
        {
            InitializeComponent();
            UpdateTitle();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeFontSizeComboBox();
            //InitializeLineNumberPanel(); 
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveFile();
        }

        
        private void UpdateTitle()
        {
            this.Text = string.IsNullOrEmpty(currentFilePath) ? "Компилятор" : Path.GetFileName(currentFilePath) ;
        }
        
        private bool ConfirmSaveChanges()
        {
            if (isTextChanged)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                    SaveFile();
                return result != DialogResult.Cancel;
            }
            return true;
        }

        
        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
                SaveFileAs(); // Если путь пуст, вызываем "Сохранить как"
            else
            {
                File.WriteAllText(currentFilePath, richTextBox1.Text);
                isTextChanged = false;
            }
        }

        
        private void SaveFileAs()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Текстовые файлы|*.txt|Все файлы|*.*" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                    currentFilePath = saveFileDialog.FileName;
                    isTextChanged = false;
                    UpdateTitle();
                }
            }
        }

        
        private void OpenFile()
        {
            if (ConfirmSaveChanges())
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Текстовые файлы|*.txt|Все файлы|*.*" })
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        richTextBox1.Text = File.ReadAllText(openFileDialog.FileName);
                        currentFilePath = openFileDialog.FileName;
                        isTextChanged = false;
                        UpdateTitle();
                    }
                }
            }
        }
        private void NewFile()
        {
            if (ConfirmSaveChanges())
            {
                richTextBox1.Clear();
                currentFilePath = string.Empty;
                isTextChanged = false;
                UpdateTitle();
            }
        }

        
        private void ExitApplication()
        {
            if (ConfirmSaveChanges())
                Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = string.Empty;
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void правкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void текстToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void сохранитьКакToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();

        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void выходToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFile();
        }
        private void InitializeFontSizeComboBox()
        {
            // Добавляем размеры шрифта в комбобокс
            comboBoxFontSize.Items.AddRange(new object[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 });

            // Устанавливаем начальный размер шрифта
            comboBoxFontSize.SelectedIndex = 4; // Например, 12

            // Подписываемся на событие изменения выбора
            comboBoxFontSize.SelectedIndexChanged += ComboBoxFontSize_SelectedIndexChanged;
        }

        private void ComboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранный размер шрифта
            if (comboBoxFontSize.SelectedItem != null)
            {
                int fontSize = (int)comboBoxFontSize.SelectedItem;

                // Применяем размер шрифта к richTextBox1 и richTextBox2
                richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, fontSize);
                //richTextBox2.Font = new Font(richTextBox2.Font.FontFamily, fontSize);
            }
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            AnalyzeCode();
            //CheckSyntax();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            
            AnalyzeCode();
            //CheckSyntax();
        }

        private void AnalyzeCode()
        {
            LexicalAnalyzer_2 analyzer = new LexicalAnalyzer_2();
            var tokens = analyzer.Analyze(richTextBox1.Text);

            StringBuilder result = new StringBuilder();
            foreach (var token in tokens)
            {
                result.AppendLine(token.ToString());
            }

            richTextBox2.Text = result.ToString();
        }

        private void CheckSyntax()
        {
            // Получаем текст из редактора
            string code = richTextBox1.Text;

            // Лексический анализ
            var lexer = new LexicalAnalyzer();
            var tokens = lexer.Analyze(code);

            // Синтаксический анализ
            var parser = new SyntaxAnalyzer();
            var errors = parser.Analyze(tokens);

            // Вывод результатов в richTextBox2
            parser.PrintResultsToRichTextBox(errors, richTextBox2);

            //// Дополнительная информация о структуре
            //if (parser.IsStructCorrect(tokens))
            //{
            //    richTextBox2.AppendText("\n\nДополнительная проверка: структура объявлена корректно.");
            //}
            //else if (errors.Count == 0)
            //{
            //    richTextBox2.AppendText("\n\nДополнительная проверка: найдены проблемы в структуре (возможно, не хватает полей или скобки не сбалансированы).");
            //}
        }
    }
}
