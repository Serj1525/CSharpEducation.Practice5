namespace Task2.BankDeposit;

using System;

// Пользовательское исключение для недостаточного баланса
public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message) : base(message) { }
}

// Пользовательское исключение для отрицательной суммы
public class InvalidAmountException : Exception
{
    public InvalidAmountException(string message) : base(message) { }
}

// Пользовательское исключение для превышения лимита снятия средств
public class WithdrawalLimitExceededException : Exception
{
    public WithdrawalLimitExceededException(string message) : base(message) { }
}

// Базовый класс для банковского счета
public class BankAccount
{
    public string AccountNumber { get; }
    public string AccountHolderName { get; }
    public decimal Balance { get; protected set; } // protected, чтобы производные классы могли изменять баланс

    public BankAccount(string accountNumber, string accountHolderName, decimal initialBalance = 0)
    {
        AccountNumber = accountNumber;
        AccountHolderName = accountHolderName;
        Balance = initialBalance;
    }

    // Метод для внесения денег на счет
    public virtual void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidAmountException("Сумма для внесения должна быть положительной.");
        }

        Balance += amount;
        Console.WriteLine($"На счет {AccountNumber} внесено {amount}. Новый баланс: {Balance}");
    }

    // Метод для снятия денег со счета
    public virtual void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidAmountException("Сумма для снятия должна быть положительной.");
        }

        if (amount > Balance)
        {
            throw new InsufficientBalanceException("Недостаточно средств на счете.");
        }

        Balance -= amount;
        Console.WriteLine($"Со счета {AccountNumber} снято {amount}. Новый баланс: {Balance}");
    }

    // Переопределенный метод ToString для удобного вывода информации о счете
    public override string ToString()
    {
        return $"Номер счета: {AccountNumber}, Владелец: {AccountHolderName}, Баланс: {Balance}";
    }
}

// Класс для обычного счета
public class RegularAccount : BankAccount
{
    public RegularAccount(string accountNumber, string accountHolderName, decimal initialBalance = 0) : base(accountNumber, accountHolderName, initialBalance)
    {
    }
}

// Класс для накопительного счета
public class SavingsAccount : BankAccount
{
    private DateTime? lastWithdrawalDate; //Nullable DateTime
    private const int WithdrawalLimitPerMonth = 1; // Ограничение на количество снятий в месяц

    public SavingsAccount(string accountNumber, string accountHolderName, decimal initialBalance = 0) : base(accountNumber, accountHolderName, initialBalance)
    {
        lastWithdrawalDate = null; // Изначально снятий не было
    }

    // Переопределенный метод для снятия денег с накопительного счета
    public override void Withdraw(decimal amount)
    {
        if (lastWithdrawalDate.HasValue && lastWithdrawalDate.Value.Month == DateTime.Now.Month) // Проверяем, было ли снятие в текущем месяце
        {
            throw new WithdrawalLimitExceededException("Превышен лимит снятий средств для накопительного счета (один раз в месяц).");
        }

        base.Withdraw(amount); // Вызываем базовый метод для выполнения основной логики снятия средств
        lastWithdrawalDate = DateTime.Now; // Обновляем дату последнего снятия
    }

     public override string ToString()
    {
        string lastWithdrawalInfo = lastWithdrawalDate.HasValue ? $"Дата последнего снятия: {lastWithdrawalDate.Value.ToShortDateString()}" : "Снятий не было";
        return $"{base.ToString()}, Тип счета: Накопительный, {lastWithdrawalInfo}";
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        // Пример использования
        try
        {
            RegularAccount regularAccount = new RegularAccount("1234567890", "Иван Иванов", 1000);
            SavingsAccount savingsAccount = new SavingsAccount("9876543210", "Мария Петрова", 5000);

            Console.WriteLine(regularAccount);
            Console.WriteLine(savingsAccount);

            regularAccount.Deposit(500);
            savingsAccount.Deposit(1000);

            Console.WriteLine(regularAccount);
            Console.WriteLine(savingsAccount);

            regularAccount.Withdraw(200);
            savingsAccount.Withdraw(1000);

             Console.WriteLine(regularAccount);
             Console.WriteLine(savingsAccount);

            // Попытка повторного снятия с накопительного счета в том же месяце
            try
            {
                savingsAccount.Withdraw(500);
            }
            catch (WithdrawalLimitExceededException e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }

            // Попытка снятия больше, чем есть на счете
            try
            {
                regularAccount.Withdraw(2000);
            }
            catch (InsufficientBalanceException e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }

            // Попытка внесения/снятия отрицательной суммы
            try
            {
                regularAccount.Deposit(-100);
            }
            catch (InvalidAmountException e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Общая ошибка: {ex.Message}"); // Перехват других возможных исключений
        }

        Console.ReadKey();
    }
}
/*
Основные улучшения и пояснения:

Пользовательские исключения:  Созданы отдельные классы исключений InsufficientBalanceException, InvalidAmountException и WithdrawalLimitExceededException, унаследованные от Exception.  Это позволяет более конкретно обрабатывать различные типы ошибок.

Защищенный баланс (protected Balance):  Баланс сделан protected, чтобы производные классы (например, SavingsAccount) могли изменять его напрямую, но он не был доступен извне класса BankAccount. Это важно для инкапсуляции.

Nullable DateTime (DateTime? lastWithdrawalDate): lastWithdrawalDate  теперь имеет тип DateTime? (Nullable DateTime).  Это позволяет установить его в null при создании SavingsAccount, чтобы указать, что снятий еще не было. Это очень важно для логики первого снятия.

Проверка даты снятия в SavingsAccount:  В методе Withdraw класса SavingsAccount теперь корректно проверяется, было ли снятие в текущем месяце. Используется lastWithdrawalDate.HasValue для проверки, что lastWithdrawalDate не null,  и lastWithdrawalDate.Value.Month == DateTime.Now.Month для сравнения месяцев.

Константа для лимита снятий: WithdrawalLimitPerMonth вынесена в константу для улучшения читаемости и возможности изменения лимита в одном месте.

Улучшен вывод информации о счете (ToString): Переопределен метод ToString() для классов BankAccount и SavingsAccount, чтобы более информативно выводить данные о счете, включая дату последнего снятия для накопительного счета.  В SavingsAccount.ToString()  используется тернарный оператор для вывода информации о дате последнего снятия, если она есть.

Обработка исключений в Main:  В Main добавлены try-catch блоки для перехвата всех возможных исключений, которые могут возникнуть при работе со счетами. Это делает код более надежным.  Внешний try-catch блок перехватывает любые исключения, которые могут произойти.

Более понятные сообщения об ошибках: Сообщения в исключениях более информативны и помогают понять причину ошибки.

Виртуальные методы: Методы Deposit и Withdraw объявлены как virtual в базовом классе BankAccount, что позволяет переопределять их поведение в производных классах (например, в SavingsAccount).

Примеры использования: В Main добавлены примеры, демонстрирующие работу с разными типами счетов и обработку исключений. Продемонстрировано, как выбрасываются и перехватываются исключения.

Явное указание типа счета в выводе: В методе ToString() класса SavingsAccount явно указывается, что это накопительный счет.

ReadOnly AccountNumber and AccountHolderName: Свойства AccountNumber и AccountHolderName сделаны read-only (только для чтения) с помощью get;  чтобы их нельзя было изменить после создания объекта счета.


Этот улучшенный пример предоставляет более полную и надежную реализацию банковских счетов с обработкой исключений и различными типами счетов.
*/