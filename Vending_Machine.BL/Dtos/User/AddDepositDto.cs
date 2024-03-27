namespace Vending_Machine.BL
{
    public class AddDepositDto
    {
        [AllowedDeposit]
        public int Deposit { get; set; }
    }
}
