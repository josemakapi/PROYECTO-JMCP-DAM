using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPV.Controlador;

namespace TPV.Modelo
{
    /// <summary>
    /// Objeto que representa un TPV en la base de datos. Contiene sólamente los datos necesarios
    /// para su identidad en la BD y en la empresa. La clase TPVBase se instancia con las propiedades
    /// extendidas para su uso operativo y no se persiste, para minimizar el tráfico de datos.
    /// </summary>
    public class TPVObj  //Se persiste
    {
        private int _id;
        public int Id { get => _id; }
        private int _numTPV;
        public int NumTPV { get => _numTPV; set => _numTPV = value; }
        private bool _isTPVMaster;
        public bool IsTPVMaster { get => _isTPVMaster; set => _isTPVMaster = value; }
        private int _codTienda;
        public int CodTienda { get => _codTienda; set => _codTienda = value; }
        private int tarifaDefecto;
        public int TarifaDefecto { get => tarifaDefecto; set => tarifaDefecto = value; }

        public TPVObj(int numTPV, bool isTPVMaster, int codTienda, int tarifaDefecto)
        {
            this._id = ControladorComun.BD!.SelectMAXInt("TPV", "_id") + 1;
            this._numTPV = numTPV;
            this._isTPVMaster = isTPVMaster;
            this._codTienda = codTienda;
            this.tarifaDefecto = tarifaDefecto;
        }
        public TPVObj(int id, int numTPV, bool isTPVMaster, int codTienda, int tarifaDefecto)
        {
            this._id = id;
            this._numTPV = numTPV;
            this._isTPVMaster = isTPVMaster;
            this._codTienda = codTienda;
            this.tarifaDefecto = tarifaDefecto;
        }

    }
}
