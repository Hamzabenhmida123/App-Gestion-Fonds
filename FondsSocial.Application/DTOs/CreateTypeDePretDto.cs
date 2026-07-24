namespace FondsSocial.Application.DTOs
{
    public class CreateTypeDePretDto
    {
        public string Code { get; set; } = null!;
        public string Libelle { get; set; } = null!;
        public int Categorie { get; set; }
        public decimal Plafond { get; set; }
        public int DureeMaxMois { get; set; }
        public int FranchiseMois { get; set; }
        public int ModeCalculFraisGestion { get; set; }
        public decimal TauxOuMontantFraisGestion { get; set; }
        public bool Actif { get; set; }
    }
}
