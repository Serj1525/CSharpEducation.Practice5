namespace CSharpEducation.Practice5;

class Program
{
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Write("Введите путь к файлу для чтения чисел: ");
                    string filePath = Console.ReadLine();

                    // Чтение чисел из файла
                    string[] lines = File.ReadAllLines(filePath);

                    if (lines.Length < 2)
                    {
                        throw new InvalidDataException("В файле должно быть как минимум два числа.");
                    }

                    // Преобразование строк в числа
                    double number1 = Convert.ToDouble(lines[0]);
                    double number2 = Convert.ToDouble(lines[1]);

                    // Выполнение деления
                    double result = DivideNumbers(number1, number2);
                    Console.WriteLine($"Результат деления {number1} на {number2} = {result}");
                    break; // Выход из цикла при успешном выполнении
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Ошибка: Файл не найден. Пожалуйста, проверьте путь к файлу и попробуйте снова.");
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ошибка: В файле содержатся некорректные данные. Пожалуйста, убедитесь, что в файле находятся два числа.");
                }
                catch (DivideByZeroException)
                {
                    Console.WriteLine("Ошибка: Деление на ноль невозможно. Пожалуйста, измените второй параметр.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        static double DivideNumbers(double num1, double num2)
        {
            if (num2 == 0)
            {
                throw new DivideByZeroException();
            }
            return num1 / num2;
        }
    }