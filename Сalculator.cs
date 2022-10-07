using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    public static class Сalculator
    {
        static double GiveValueVariadle(string name)
        {
            double value = 0;

            for (int i = 0; i < MainWindow.NameTableList.Count; i++)
            {
                if (name == MainWindow.NameTableList[i].name)
                {
                    value = MainWindow.NameTableList[i].value;
                    break;
                }
            }

            return value;
        }

        static double Prim(Parser parser)
        {
            try
            {
                parser.get_lexem();

                switch (parser.get_last().type)
                {
                    case LexemType.LT_Number:
                        {
                            double v = parser.get_last().value;
                            parser.get_lexem();
                            return v;
                        }

                    case LexemType.LT_CosSqrtExp:
                        {
                            switch (parser.get_last().name)
                            {
                                case "exp":
                                    return Math.Exp(ForPower(parser));
                                case "sqrt":
                                    return Math.Sqrt(ForPower(parser));
                                case "cos":
                                    return Math.Cos(ForPower(parser) * Math.PI / 180.0);
                                case "sin":
                                    return Math.Sin(ForPower(parser) * Math.PI / 180.0);
                                default:
                                    throw new Exception("Какой-то из символов не распознан");
                            }
                        }
                    case LexemType.LT_Identifier:
                        {
                            string name = parser.get_last().name;
                            parser.get_lexem();
                            return GiveValueVariadle(name);
                        }
                    case LexemType.LT_Delimiter:
                        switch (parser.get_last().delimiter)
                        {
                            case '-':
                                return -Prim(parser);
                            case '(':
                                {
                                    double e = Math.Round(EvaluateExpression(parser),3);
                                    if (parser.get_last().type == LexemType.LT_Delimiter && parser.get_last().delimiter == ')')
                                    {
                                        parser.get_lexem();
                                        return e;
                                    }
                                    else
                                      throw new Exception("Не хватает ещё одной \')\' " + parser.get_last().delimiter + parser.get_lexem().delimiter);

                                }
                            default:
                                throw new Exception("Какой-то из символов не распознан");
                        }
                    default:
                        throw new Exception("Какой-то из символов не распознан");
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        // Обрабатывает возведение в степень
        static double ForPower(Parser parser)
        {
            double left = Prim(parser);
            for (; ; )
            {
                if (parser.get_last().type == LexemType.LT_Delimiter)
                {
                    switch (parser.get_last().delimiter)
                    {
                        case '^':
                            left = Math.Pow(left, Prim(parser));
                            break;
                        default:
                            return left;
                    }
                }
                else
                    return left;
            }
        }

        // Обрабатывает корень, экспоненту и косинус
        static double SqrtExpCos(Parser parser)
        {
            double left = ForPower(parser);
            for (; ; )
            {
                //if (parser.get_last().type == LexemType.LT_CosSqrtExp)
                //{
                //    switch (parser.get_last().name)
                //    {
                //        case "exp":
                //            left = Math.Exp(ForPower(parser));
                //            break;
                //        case "sqrt":
                //            left = Math.Sqrt(ForPower(parser));
                //            break;
                //        case "cos":
                //            left = Math.Cos(ForPower(parser));
                //            break;
                //        case "sin":
                //            left = Math.Sin(ForPower(parser));
                //            break;
                //        default:
                //            return left;
                //    }
                //}
                //else
                    return left;
            }
        }


        // Обрабатывает умножение и деление
        static double Term(Parser parser)
        {
            double left = SqrtExpCos(parser);
            try
            {
                for (; ; )
                {
                    if (parser.get_last().type == LexemType.LT_Delimiter)
                    {
                        switch (parser.get_last().delimiter)
                        {
                            case '*':
                                left *= ForPower(parser);
                                break;
                            case '/':
                                {
                                    const double precision = 1.0e-5f;
                                    double d = ForPower(parser);
                                    if (Math.Abs(d) > precision)
                                    {
                                        left /= d;
                                    }
                                    else
                                        throw new Exception("Деление на 0 недопустимо");
                                }
                                break;
                            default:
                                return left;
                        }
                    }
                    else
                        return left;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        // Обрабатывает сложение и вычитание
        static double PlusMinus(Parser parser)
        {
            double left = Term(parser);

            for (; ; )
            {
                if (parser.get_last().type == LexemType.LT_Delimiter)
                {
                    switch (parser.get_last().delimiter)
                    {
                        case '+':
                            left += Term(parser);
                            break;
                        case '-':
                            left -= Term(parser);
                            break;
                        default:
                            return left;
                    }
                }
                else
                    return left;
            }
        }

        public static double EvaluateExpression(Parser parser)
        {
            double left = PlusMinus(parser);
            return left;
        }
    }
}
