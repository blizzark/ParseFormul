using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
   
    // Типы лексем, используемых калькулятором
    public enum LexemType
    {
        LT_Unknown,
        LT_Number,
        LT_End,
        LT_Delimiter,
        LT_Identifier,
        LT_CosSqrtExp,
        LT_EOL,
    };
    // Структура лексемы
    public struct Lexem
    {
        public LexemType type;         // тип
        public double value;        // значение    (если тип TT_Number)
        public char delimiter;    // разделитель (если тип TT_Delimiter)
        public string name;         // имя         (если тип TT_Identifier)
    };


    public class Parser
    {
       
        private string text = "";
        private int numStr = 0;
        Lexem last;
        public Parser(string text)
        {
          
            this.text = text.Replace(" ", "");
            this.text = this.text.Replace("–","-");

        }
        // Возвращает предыдущую считанную лексему
        // Если не было считано никакой лексемы - вызывает get_token
        // для чтения лексемы
        public Lexem get_last()
        {
            if (last.type == LexemType.LT_Unknown)
                return get_lexem();
            return last;
        }
        // Чтение лексемы из потока
        public Lexem get_lexem() {
            if (numStr < text.Length)
            {
                //Берём из строки символ
                char c = text[numStr];
             
                
                //разделитель
                const string delimiters = "+-*/()=<>^";
                for (int i = 0; i < delimiters.Length; i++)
                {
                    if(c == delimiters[i])
                    {
                        last.type = LexemType.LT_Delimiter;
                        last.delimiter = c;
                        numStr++; // итерация по элементу строки
                        return last;
                    }
                }

                if (Char.IsNumber(c) || c == '.' || c == ',')
                {
                    string value = "";
                    while (Char.IsNumber(text[numStr]) || text[numStr] == '.' || text[numStr] == ',')
                    {
                       
                        value += text[numStr];
                        numStr++;
                        if (numStr >= text.Length)
                        {
                            break;
                        }
                    }
                    last.value = Convert.ToDouble(value);
                    last.type = LexemType.LT_Number;
                    return last;
                }

                if (Char.IsLetter(c))
                {
                    last.name = "";
                    while (Char.IsNumber(text[numStr]) || Char.IsLetter(text[numStr]))
                    {
                       
                        last.name += text[numStr];
                        numStr++;
                        if (numStr >= text.Length)
                        {
                            break;
                        }
                    }
                    if(last.name == "cos" || last.name == "sqrt" || last.name == "exp" || last.name == "sin")
                        last.type = LexemType.LT_CosSqrtExp;
                    else
                        last.type = LexemType.LT_Identifier;
                    return last;
                }


               
            }
            else
            {
                last.type = LexemType.LT_End;
                return last;
            }
            return last;
        }

       
    }
}
