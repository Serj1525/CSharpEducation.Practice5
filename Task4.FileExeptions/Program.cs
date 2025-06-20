namespace Task4.FileExeptions;

using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        string filePath = string.Empty;
        bool fileReadSuccessfully = false;

        while (!fileReadSuccessfully)
        {
            Console.Write("Введите путь к файлу: ");
            filePath = Console.ReadLine();

            try
            {
                // Попробуем прочитать содержимое файла
                string[] lines = File.ReadAllLines(filePath);
                Console.WriteLine("Содержимое файла:");
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
                fileReadSuccessfully = true; // Успешно прочитали файл
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Ошибка: Файл не найден. Пожалуйста, проверьте путь и попробуйте снова.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: У вас нет прав доступа к этому файлу. Пожалуйста, выберите другой файл.");
            }
            catch (IOException ex) when (IsFileLocked(ex))
            {
                Console.WriteLine("Ошибка: Файл заблокирован другой программой. Попробуйте снова через несколько секунд или выберите другой файл.");
                Thread.Sleep(5000); // Ждем 5 секунд перед повторной попыткой
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}. Пожалуйста, проверьте файл и попробуйте снова.");
            }
        }
    }

    // Метод для проверки, заблокирован ли файл
    private static bool IsFileLocked(IOException exception)
    {
        // Проверяем, является ли исключение связано с блокировкой файла
        return exception.Message.Contains("используется другим процессом");
    }
}

/*
Объяснение кода:
Основной цикл: Программа запрашивает у пользователя путь к файлу до тех пор, пока файл не будет успешно прочитан.
Обработка исключений:
FileNotFoundException: Обрабатывает случай, когда файл не найден.
UnauthorizedAccessException: Обрабатывает ситуацию, когда у пользователя нет прав доступа к файлу.
IOException: Проверяет, заблокирован ли файл другой программой. Если да, то программа ждет 5 секунд и предлагает пользователю попробовать снова.
Общий catch: Обрабатывает любые другие исключения, выводя сообщение об ошибке.
Метод IsFileLocked: Проверяет, связано ли исключение с блокировкой файла.
Использование:
Скопируйте код в файл с расширением .cs и запустите его в среде разработки C#.
Программа будет запрашивать путь к файлу, пока не удастся его прочитать, обрабатывая возможные ошибки.
*/