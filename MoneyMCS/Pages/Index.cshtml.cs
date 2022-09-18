using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MoneyMCS.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public InputModel CalculatorInput { get; set; }
        
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            CalculatorInput.Shortfall = (CalculatorInput.Assets + CalculatorInput.Receivables) - CalculatorInput.Liabilities;
            return Page();
        }

        public class InputModel
        {
            [BindRequired]
            public decimal Assets { get; set; }
            [BindRequired]
            public decimal Receivables { get; set; }
            [BindRequired]

            public decimal Liabilities { get; set; }
            [BindRequired]

            public decimal Shortfall { get; set; }
        }
    }
}
