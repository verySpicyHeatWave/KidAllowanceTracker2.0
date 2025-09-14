using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Models
{
    public class AllowancesViewModel
    {
        public List<SinglePointModel> PointList { get; set; } = [];
        public double TotalAllowance => CalculateTotal();


        public AllowancesViewModel(List<AllowancePointDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                PointList.Add(new SinglePointModel(dto));
            }
        }

        public int GetPoints(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Points ?? -1;

        public int GetPrice(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Price ?? -1;


        private double CalculateTotal()
        {
            int resp = PointList.Sum(t => t.Total);
            return resp / 100.0;
        }
    }
}
