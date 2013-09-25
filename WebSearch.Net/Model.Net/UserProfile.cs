using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Maths.Net;
using WebSearch.Common.Net;

namespace WebSearch.Model.Net
{
    public class UserProfile : BaseModel
    {
        #region Properties

        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Gender _gender;

        public Gender Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private int _age;

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        private Nationality _nation;

        public Nationality Nation
        {
            get { return _nation; }
            set { _nation = value; }
        }

        #endregion
    }

    #region Class Gender

    public class Gender
    {
        public static Gender GetGender(int id, float degree)
        {
            Gender gender = (Gender)id;
            gender.Degree.Value = degree;
            return gender;
        }
        public static Gender GetGender(string name, float degree)
        {
            Gender gender = (Gender)name;
            gender.Degree.Value = degree;
            return gender;
        }

        public static Gender Male
        {
            get { return new Gender(1); }
        }
        public static Gender GetMale(float degree)
        {
            return new Gender(1, degree);
        }

        public static Gender Female
        {
            get { return new Gender(2); }
        }
        public static Gender GetFemale(float degree)
        {
            return new Gender(2, degree);
        }

        public static Gender Neuter
        {
            get { return new Gender(3); }
        }
        public static Gender GetNeuter(float degree)
        {
            return new Gender(3, degree);
        }

        public static Gender Invalid
        {
            get { return new Gender(Const.Invalid); }
        }

        private Gender(int id)
        {
            this._id = id;
        }
        private Gender(int id, float degree) : this(id)
        {
            this._degree.Value = degree;
        }

        #region Properties

        private int _id;

        private Degree _degree = Degree.Normal;

        public Degree Degree
        {
            get { return _degree; }
            set { _degree = value; }
        }

        public bool IsNeuter
        {
            get { return _id == 3; }
        }
        public bool IsMale
        {
            get { return _id == 1; }
        }
        public bool IsFemale
        {
            get { return _id == 2; }
        }
        public bool IsValid
        {
            get { return _id != Const.Invalid; }
        }

        #endregion

        #region Overrided Operators

        public static bool operator ==(Gender g1, Gender g2)
        {
            return (g1._id == g2._id);
        }

        public static bool operator !=(Gender g1, Gender g2)
        {
            return (g1._id != g2._id);
        }

        public override bool Equals(object obj)
        {
            if (obj is Gender)
                return (this._id == ((Gender)obj)._id);

            return false;
        }

        public override string ToString()
        {
            switch (_id)
            {
                case 1: return "Male";
                case 2: return "Female";
                case 3: return "Neuter";
                default: return "Invalid";
            }
        }

        public static explicit operator Gender(int id)
        {
            switch (id)
            {
                case 1:
                    return Gender.Male;
                case 2:
                    return Gender.Female;
                case 3:
                    return Gender.Neuter;
                default:
                    return Gender.Invalid;
            }
        }

        public static explicit operator Gender(string name)
        {
            name = name.Trim().ToLower();
            if (name.StartsWith("m"))
                return Gender.Male;
            if (name.StartsWith("f"))
                return Gender.Female;
            if (name.StartsWith("n"))
                return Gender.Neuter;
            return Gender.Invalid;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    #endregion

    public enum Nationality
    {
        Achinese, Akans, Albanians, Algerians, Alsatians, Americans, Amharas,
        Anglo_Australians, Anglo_Canadians, Anglo_New_Zealanders, Arabians, 
        Argentineans, Armenians, Assamese, Austrians, Aymaras, Azandes,
        Aztecs, Azerbaijanians, Bagirmis, Balinese, Baluchis, Bambundus,
        Bamilekes, Bandas, Banjars, Bashkirs, Basques, Batekes, Battaks,
        Bejas, Bembas, Bengalis, Bhilis, Bicols, Bielorussians, Biharis,
        Binis, Boers, Bolivians, Bosnians, Brazilians, Bretons, Bugis,
        Bulgarians, Buras, Burmese, Catalans, Chambas, Chileans, Chinese,
        Chokwes, Chuvash, Colombians, Congos, Costa_Ricans, Croats, Cubans,
        Czechs, Danes, Dinkas, Dominicans, Ecuadorians, Egyptians, English,
        Estonians, Ewes, Fangs, Finns, Flamands, Fons, French, French_Candaians,
        French_Swiss, Fulbes, Gallas, Galicians, Gandas, Georgians, Germans,
        German_Swiss, Gilians, Gonds, Greeks, Guatemalians, Gujaratis, Gypsies,
        Haitians, Hausas, Hazaras, Hehes, Hindustanis, Hollanders, Hondurasans,
        Hungarians, Ibibios, Ibos, Ilokos, Iraqans, Irish, Italians, Jamaicans,
        Japanese, Javanese, Jews, Joluos, Jordanese, Jukuns, Kabyles, Kalenjins,
        Kambas, Kannarese, Kanuris, Karens, Kashmiris, Kazakhs, Khalkha_Mongolians,
        Khmers, Kikuyus, Kirghiz, Kisis, Koreans, Krus, Kurds, Kuwaitians,
        Lahndas, Laotians, Latvians, Lebanese, Libyans, Lithuanians, Lubas,
        Luhiyas, Lurs, Macassarese, Macedonians, Maduras, Makondes, Makuas,
        Malagasy, Malawis, Malays, Malayalams, Mandingos, Marathis, Mauritanians,
        Mazenderans, Mendes, Mexicans, Minangkabaus, Moldavians, Mongos, Moru_Mangbetus,
        Mosis, Moroccans, Mundas, Nepalese, Ngalas, Nicaraguans, Norwegians,
        Nubians, Nupes, Nyaruandas, Nyamwezis, Nyoros, Oraons, Oriyas,
        Ovimbundus, Paharis, Pampangans, Panamans, Pangasinans, Paraguans, Pedis,
        Persians, Peruvians, Polish, Portuguese, Puerto_Ricans, Punjabis, Pushtus,
        Quechuans, Rajasthanis, Rifs, Rumanians, Rundis, Russians, Salvadorians,
        Santalis, Sasaks, Scotch, Senufos, Serbs, Shans, Shonas, Shluhs,
        Siamese, Sindhis, Singhalese, Slovaks, Slovenians, Somalis, Songhais,
        Southern_Luos, Spanish, Sudanese, Sundanese, Sutos, Swahilis, Swedes,
        Syrians, Tagalogs, Tajiks, Tamazigts, Tamils, Tatars, Telugus,
        Tesos, Thongas, Tigrais, Tivs, Toradjas, Turks, Tunisians,
        Turkmen, Tswanas, Tulus, Ukraninians, Uruguans, Uygur, Uzbeks,
        Venezuelians, Vietnamese, Visayans, Walloons, Wayaos, Welsh, Wolofs,
        Xosas, Yemenese, Yorubas, Zulus
    }
}
