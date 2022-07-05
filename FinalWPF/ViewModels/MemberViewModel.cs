using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.SQLite;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace FinalWPF.ViewModels
{
    public class MemberViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// True : Member Profile Page
        /// False : Member Register Page
        /// </summary>
        private bool changePageVisibility = true;
        public bool ChangePageVisibility
        {
            get
            {
                return changePageVisibility;
            }
            set
            {
                changePageVisibility = value;
                OnPropertyChanged("ChangePageVisibility");
            }
        }

        /// <summary>
        /// True : Modify TextBox Visible
        /// False : Modify TextBox Collapse
        /// </summary>
        private bool modifyBoxVisibility = true;
        public bool ModifyBoxVisibility
        {
            get
            {
                return modifyBoxVisibility;
            }
            set
            {
                modifyBoxVisibility = value;
                OnPropertyChanged("ModifyBoxVisibility");
            }
        }

        /// <summary>
        /// True : Member Register Page 에 있을 때
        /// False : Member Profile Page 에 있을 때
        /// </summary>
        private bool isCheckRegisterPage = false;
        public bool IsCheckRegisterPage
        {
            get
            {
                return isCheckRegisterPage;
            }
            set
            {
                isCheckRegisterPage = value;
                OnPropertyChanged("IsCheckRegisterPage");
            }
        }

        /// <summary>
        /// True : 수정중일 때
        /// False : 수정하지 않을 때
        /// </summary>
        private bool isCheckModifying = false;
        public bool IsCheckModifying
        {
            get
            {
                return isCheckModifying;
            }
            set
            {
                isCheckModifying = value;
                OnPropertyChanged("IsCheckModifying");
            }
        }

        /// <summary>
        /// Member Profile Page 에서 "정보를 보여주는 부분"에 Binding 됨
        /// </summary>
        private Group.Member seenMember = new Group.Member();
        public Group.Member SeenMember
        {
            get
            {
                return seenMember;
            }
            set
            {
                seenMember = value;
                OnPropertyChanged("SeenMember");
            }
        }

        /// <summary>
        /// Member Register Page 에서 "정보를 입력하는 부분"에 Binding 됨
        /// </summary>
        private Group.Member newMember = new Group.Member();
        public Group.Member NewMember
        {
            get
            {
                return newMember;
            }
            set
            {
                newMember = value;
                OnPropertyChanged("NewMember");
            }
        }

        /// <summary>
        /// Member Register Page 에서 "정보를 수정하는 부분"에 Binding 됨
        /// </summary>
        private Group.Member originalDataMember = new Group.Member();
        public Group.Member OriginalDataMember
        {
            get
            {
                return originalDataMember;
            }
            set
            {
                originalDataMember = value;
                OnPropertyChanged("OriginalDataMember");
            }
        }

        /// <summary>
        /// 연령대에 따른 UserGroup 생성
        /// </summary>
        private ObservableCollection<Group> usersGroup = new ObservableCollection<Group>();
        public ObservableCollection<Group> UsersGroup
        {
            get
            {
                return usersGroup;
            }
            set
            {
                usersGroup = value;
                OnPropertyChanged("UsersGroup");
            }
        }

        /// <summary>
        /// Member로 구성된 각 Group의 이름들
        /// </summary>
        private ObservableCollection<Group.Member> membersGroupUnderTen = new ObservableCollection<Group.Member>();
        public ObservableCollection<Group.Member> MembersGroupUnderTen
        {
            get
            {
                return membersGroupUnderTen;
            }
            set
            {
                membersGroupUnderTen = value;
                OnPropertyChanged("MembersGroupUnderTen");
            }
        }

        private ObservableCollection<Group.Member> membersGroupTwenty = new ObservableCollection<Group.Member>();
        public ObservableCollection<Group.Member> MembersGroupTwenty
        {
            get
            {
                return membersGroupTwenty;
            }
            set
            {
                membersGroupTwenty = value;
                OnPropertyChanged("MembersGroupTwenty");
            }
        }

        private ObservableCollection<Group.Member> membersGroupThirty = new ObservableCollection<Group.Member>();
        public ObservableCollection<Group.Member> MembersGroupThirty
        {
            get
            {
                return membersGroupThirty;
            }
            set
            {
                membersGroupThirty = value;
                OnPropertyChanged("MembersGroupThirty");
            }
        }

        private ObservableCollection<Group.Member> membersGroupOverFourty = new ObservableCollection<Group.Member>();
        public ObservableCollection<Group.Member> MembersGroupOverFourty
        {
            get
            {
                return membersGroupOverFourty;
            }
            set
            {
                membersGroupOverFourty = value;
                OnPropertyChanged("MembersGroupOverFourty");
            }
        }

        /// <summary>
        /// 연령대별 User Group 에 대한 Class
        /// </summary>
        public class Group : INotifyPropertyChanged
        {
            /// <summary>
            /// Nav User Group 을 나이대 별로 구분하기 위한 그룹 이름
            /// </summary>
            public string UserGroupTitle { get; set; }

            /// <summary>
            /// Nav User Group 안에 몇명의 사람이 있는지 미리 표시하기 위함
            /// </summary>
            public int PeopleCount { get; set; }

            /// <summary>
            /// Nav Group Button 을 숨겨 놓기 위한 Visibility 속성
            /// </summary>
            private bool navGroupButtonVisibility = true;
            public bool NavGroupButtonVisibility
            {
                get
                {
                    return navGroupButtonVisibility;
                }
                set
                {
                    navGroupButtonVisibility = value;
                    OnPropertyChanged("NavGroupButtonVisibility");
                }
            }

            /// <summary>
            /// 각 그룹에 Member 를 추가하는 과정
            /// </summary>
            private ObservableCollection<Member> usersMember = new ObservableCollection<Member>();
            public ObservableCollection<Member> UsersMember
            {
                get
                {
                    return usersMember;
                }
                set
                {
                    usersMember = value;
                    OnPropertyChanged("UsersMember");
                }
            }

            /// <summary>
            /// Nav 각 그룹 안에 멤버들의 Class
            /// </summary>
            public class Member : INotifyPropertyChanged, IDataErrorInfo
            {
                private string memberName;
                public string MemberName
                {
                    get
                    {
                        return memberName;
                    }
                    set
                    {
                        memberName = value;
                        OnPropertyChanged("MemberName");
                    }
                }

                private string memberGender;
                public string MemberGender
                {
                    get
                    {
                        return memberGender;
                    }
                    set
                    {
                        memberGender = value;
                        OnPropertyChanged("MemberGender");
                    }
                }

                private bool memberMan;
                public bool MemberMan
                {
                    get
                    {
                        return memberMan;
                    }
                    set
                    {
                        memberMan = value;
                        OnPropertyChanged("MemberMan");
                    }
                }

                private bool memberWoman;
                public bool MemberWoman
                {
                    get
                    {
                        return memberWoman;
                    }
                    set
                    {
                        memberWoman = value;
                        OnPropertyChanged("MemberWoman");
                    }
                }

                private bool memberNothing;
                public bool MemberNothing
                {
                    get
                    {
                        return memberNothing;
                    }
                    set
                    {
                        memberNothing = value;
                        OnPropertyChanged("MemberNothing");
                    }
                }

                private string memberAge;
                public string MemberAge
                {
                    get
                    {
                        return memberAge;
                    }
                    set
                    {
                        memberAge = value;
                        OnPropertyChanged("MemberAge");
                    }
                }

                private string memberBirth;
                public string MemberBirth
                {
                    get
                    {
                        return memberBirth;
                    }
                    set
                    {
                        memberBirth = value;
                        OnPropertyChanged("MemberBirth");
                    }
                }

                private string memberShowBirth;
                public string MemberShowBirth
                {
                    get
                    {
                        return memberShowBirth;
                    }
                    set
                    {
                        memberShowBirth = value;
                        OnPropertyChanged("MemberShowBirth");
                    }
                }

                private string memberPhone;
                public string MemberPhone
                {
                    get
                    {
                        return memberPhone;
                    }
                    set
                    {
                        memberPhone = value;
                        OnPropertyChanged("MemberPhone");
                    }
                }

                private string memberAddress;
                public string MemberAddress
                {
                    get
                    {
                        return memberAddress;
                    }
                    set
                    {
                        memberAddress = value;
                        OnPropertyChanged("MemberAddress");
                    }
                }

                private string memberElse;
                public string MemberElse
                {
                    get
                    {
                        return memberElse;
                    }
                    set
                    {
                        memberElse = value;
                        OnPropertyChanged("MemberElse");
                    }
                }

                private bool memberSelected = false;
                public bool MemberSelected
                {
                    get
                    {
                        return memberSelected;
                    }
                    set
                    {
                        memberSelected = value;
                        OnPropertyChanged("MemberSelected");
                    }
                }

                private int memberClassify;
                public int MemberClassify
                {
                    get
                    {
                        return memberClassify;
                    }
                    set
                    {
                        memberClassify = value;
                        OnPropertyChanged("MemberClassify");
                    }
                }

                private bool memberNull = false;
                public bool MemberNull
                {
                    get
                    {
                        return memberNull;
                    }
                    set
                    {
                        memberNull = value;
                        OnPropertyChanged("MemberNull");
                    }
                }

                public string Error
                {
                    get
                    {
                        return null;
                    }
                }

                public string this[string columnName]
                {
                    get
                    {
                        string result = null;

                        if (columnName == "MemberName")
                        {
                            if (!Regex.IsMatch(this.MemberName, @"[가-힣]{2,5}"))
                            {
                                result = "이름을 2~5글자의 한글로 입력하세요 \n예시)김철수";
                            }
                        }
                        else if (columnName == "MemberPhone")
                        {
                            if (!Regex.IsMatch(this.MemberPhone, @"[0-9]{2,3}-[0-9]{3,4}-[0-9]{4}"))
                            {
                                result = "번호를 올바르게 입력하세요 \n예시)010-1111-1111";
                            }
                        }
                        else if (columnName == "MemberAddress")
                        {
                            if (this.MemberAddress == "")
                            {
                                result = "거주지를 입력하세요";
                            }
                        }
                        else if (columnName == "MemberBirth")
                        {
                            if (!Regex.IsMatch(this.MemberBirth, @"/^[0-9]+/g") && this.MemberBirth.Length < 8)
                            {
                                result = "생년월일을 8자리 숫자로 입력하세요 \n예시)19990101";
                            }
                        }
                        return result;
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;
                protected void OnPropertyChanged(string propertyName)
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Command 정리
        /// </summary>
        public ICommand HeaderProfileButtonCommand { get; set; }
        public ICommand HeaderRegisterButtonCommand { get; set; }
        public ICommand NavGroupButtonCommand { get; set; }
        public ICommand NavMemberButtonCommand { get; set; }
        public ICommand ModifyButtonCommand { get; set; }
        public ICommand ModifyCompleteButtonCommand { get; set; }
        public ICommand ModifyCancelButtonCommand { get; set; }
        public ICommand RemoveButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ICommand ResetButtonCommand { get; set; }

        public MemberViewModel()
        {
            // DB 생기면 없어질 부분
            MembersGroupUnderTen.Add(new Group.Member() { MemberName = "이강희", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "20050326", MemberShowBirth = "2005년 03월 26일", MemberAge = "17살", MemberClassify = 1, MemberPhone = "010-9078-0632", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 행복관", MemberElse = "알리오 올리오를 좋아합니다.", MemberSelected = true });
            MembersGroupUnderTen.Add(new Group.Member() { MemberName = "박보영", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "20080507", MemberShowBirth = "2008년 05월 07일", MemberAge = "14살", MemberClassify = 1, MemberPhone = "010-1234-0000", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 창조관", MemberElse = "피자를 좋아합니다.", MemberSelected = false });
            MembersGroupUnderTen.Add(new Group.Member() { MemberName = "정우성", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "20041111", MemberShowBirth = "2004년 11월 11일", MemberAge = "18살", MemberClassify = 1, MemberPhone = "010-2345-1111", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 은혜관", MemberElse = "굽네치킨을 좋아합니다.", MemberSelected = false });
            MembersGroupUnderTen.Add(new Group.Member() { MemberName = "강한나", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "20130106", MemberShowBirth = "2013년 01월 06일", MemberAge = "9살", MemberClassify = 1, MemberPhone = "010-3456-2222", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 비전관", MemberElse = "항정살을 좋아합니다.", MemberSelected = false });


            MembersGroupTwenty.Add(new Group.Member() { MemberName = "이강희", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "19970326", MemberShowBirth = "1997년 03월 26일", MemberAge = "25살", MemberClassify = 2, MemberPhone = "010-9078-0632", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 행복관", MemberElse = "알리오 올리오를 좋아합니다.", MemberSelected = false });
            MembersGroupTwenty.Add(new Group.Member() { MemberName = "박보영", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "19950507", MemberShowBirth = "1995년 05월 07일", MemberAge = "27살", MemberClassify = 2, MemberPhone = "010-1234-0000", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 창조관", MemberElse = "피자를 좋아합니다.", MemberSelected = false });
            MembersGroupTwenty.Add(new Group.Member() { MemberName = "정우성", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "19991111", MemberShowBirth = "1999년 11월 11일", MemberAge = "23살", MemberClassify = 2, MemberPhone = "010-2345-1111", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 은혜관", MemberElse = "굽네치킨을 좋아합니다.", MemberSelected = false });
            MembersGroupTwenty.Add(new Group.Member() { MemberName = "강한나", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "20000106", MemberShowBirth = "2000년 01월 06일", MemberAge = "22살", MemberClassify = 2, MemberPhone = "010-3456-2222", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 비전관", MemberElse = "항정살을 좋아합니다.", MemberSelected = false });
            MembersGroupTwenty.Add(new Group.Member() { MemberName = "송중기", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "19991210", MemberShowBirth = "1999년 12월 10일", MemberAge = "23살", MemberClassify = 2, MemberPhone = "010-9876-3333", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 로뎀관", MemberElse = "닭강정을 좋아합니다.", MemberSelected = false });
            MembersGroupTwenty.Add(new Group.Member() { MemberName = "채수빈", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "19980820", MemberShowBirth = "1998년 08월 20일", MemberAge = "24살", MemberClassify = 2, MemberPhone = "010-8765-4444", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 갈대상자관", MemberElse = "참치를 좋아합니다.", MemberSelected = false });

            MembersGroupThirty.Add(new Group.Member() { MemberName = "이강희", MemberMan = true, MemberWoman = false, MemberNothing = false, MemberGender = "남자", MemberBirth = "19880326", MemberShowBirth = "1988년 03월 26일", MemberAge = "34살", MemberClassify = 3, MemberPhone = "010-9078-0632", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 행복관", MemberElse = "알리오 올리오를 좋아합니다.", MemberSelected = false });
            MembersGroupThirty.Add(new Group.Member() { MemberName = "박보영", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "19850507", MemberShowBirth = "1985년 05월 07일", MemberAge = "37살", MemberClassify = 3, MemberPhone = "010-1234-0000", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 창조관", MemberElse = "피자를 좋아합니다.", MemberSelected = false });

            MembersGroupOverFourty.Add(new Group.Member() { MemberName = "박보영", MemberMan = false, MemberWoman = true, MemberNothing = false, MemberGender = "여자", MemberBirth = "19760507", MemberShowBirth = "1976년 05월 07일", MemberAge = "46살", MemberClassify = 4, MemberPhone = "010-1234-0000", MemberAddress = "포항시 북구 흥해읍 남송리 3번지 한동대학교 창조관", MemberElse = "피자를 좋아합니다.", MemberSelected = false });

            UsersGroup.Add(new Group() { UserGroupTitle = "10대이하", UsersMember = MembersGroupUnderTen, PeopleCount = MembersGroupUnderTen.Count });
            UsersGroup.Add(new Group() { UserGroupTitle = "20대", UsersMember = MembersGroupTwenty, PeopleCount = MembersGroupTwenty.Count });
            UsersGroup.Add(new Group() { UserGroupTitle = "30대", UsersMember = MembersGroupThirty, PeopleCount = MembersGroupThirty.Count });
            UsersGroup.Add(new Group() { UserGroupTitle = "40대이상", UsersMember = MembersGroupOverFourty, PeopleCount = MembersGroupOverFourty.Count });

            // 첫 화면 default 값(임의)
            AfterRemoveShowContents(MembersGroupUnderTen);

            NewMember.MemberName = "";
            NewMember.MemberPhone = "";
            NewMember.MemberBirth = "";

            HeaderProfileButtonCommand = new Command(HeaderProfileButtonExecuteMethod, HeaderProfileButtonCanExecuteMethod);
            HeaderRegisterButtonCommand = new Command(HeaderRegisterButtonExecuteMethod, HeaderRegisterButtonCanExecuteMethod);
            NavGroupButtonCommand = new Command(NavGroupButtonExecuteMethod, NavGroupButtonCanExecuteMethod);
            NavMemberButtonCommand = new Command(NavMemberButtonExecuteMethod, NavMemberButtonCanExecuteMethod);
            ModifyButtonCommand = new Command(ModifyButtonExecuteMethod, ModifyButtonCanExecuteMethod);
            ModifyCompleteButtonCommand = new Command(ModifyCompleteButtonExecuteMethod, ModifyCompleteButtonCanExecuteMethod);
            ModifyCancelButtonCommand = new Command(ModifyCancelButtonExecuteMethod, ModifyCancelButtonCanExecuteMethod);
            RemoveButtonCommand = new Command(RemoveButtonExecuteMethod, RemoveButtonCanExecuteMethod);
            RegisterButtonCommand = new Command(RegisterButtonExecuteMethod, RegisterButtonCanExecuteMethod);
            CancelButtonCommand = new Command(CancelButtonExecuteMethod, CancelButtonCanExecuteMethod);
            ResetButtonCommand = new Command(ResetButtonExecuteMethod, ResetButtonCanExecuteMethod);
        }

        /// <summary>
        /// Header 에서 Member Profile 버튼을 클릭시 발생하는 Command
        /// - Profile Page 로 이동
        /// - Member Register Page 에서 Member Profile 버튼을 클릭시 입력을 그만둘건지 경고 메세지 표시
        /// </summary>
        /// <param name="obj"></param>
        private void HeaderProfileButtonExecuteMethod(object obj)
        {
            if (IsCheckRegisterPage)
            {
                if (MessageBox.Show("작성을 취소하시겠습니까?", "취소", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    IsCheckRegisterPage = false;
                    ChangePageVisibility = true;
                    ModifyBoxVisibility = true;
                }
            }
        }
        private bool HeaderProfileButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// Header 에서 Member Register 버튼을 클릭시 발생하는 Command
        /// - Register Page 로 이동
        /// - Register Page 에서 작성중이던 NewMember Data 초기화
        /// </summary>
        /// <param name="obj"></param>
        private void HeaderRegisterButtonExecuteMethod(object obj)
        {
            if (IsCheckModifying)
            {
                if (MessageBox.Show("수정을 취소하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SeenMember = OriginalDataMember;
                    ModifyBoxVisibility = !ModifyBoxVisibility;
                    IsCheckModifying = false;
                }
            }
            IsCheckRegisterPage = true;
            ChangePageVisibility = false;
            ResetContents(NewMember);
            NewMember.MemberNothing = true;
            IsCheckModifying = false;
        }
        private bool HeaderRegisterButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// Nav Group 버튼 클릭시 실행되는 Command
        /// - Selected Group Button 과 Unselected Group Button 의 Visibility Toggle
        /// </summary>
        /// <param name="obj"></param>
        private void NavGroupButtonExecuteMethod(object obj)
        {
            var user = obj as Group;
            for (var i = 0; i < UsersGroup.Count; i++)
            {
                if (UsersGroup[i].UserGroupTitle == user.UserGroupTitle)
                {
                    if (user.NavGroupButtonVisibility == false)
                    {
                        user.NavGroupButtonVisibility = true;
                    }
                    else
                    {
                        user.NavGroupButtonVisibility = false;
                    }
                }
                else
                {
                    UsersGroup[i].NavGroupButtonVisibility = true;
                }
            }
        }
        private bool NavGroupButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// Nav Member 버튼 클릭시 실행되는 Command
        /// - 전체 Nav Member Button 의 Selected 초기화
        /// - 내가 선택한 object 의 Button 만 Selected 활성화
        /// - Selected 된 Member 의 데이터 SeenMember 에서 보여주기
        /// </summary>
        /// <param name="obj"></param>
        private void NavMemberButtonExecuteMethod(object obj)
        {
            ResetMemberSelected();
            ShowContents(obj);
        }
        private bool NavMemberButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 수정하기 버튼 클릭시 실행되는 Command
        /// - 수정할 수 있는 상태인지 Check 후 경고창 표시
        /// - 이미 입력되있던 정보 Clone 하여 CopyMember 에 저장(수정 취소 시 데이터를 불러오기 위함)
        /// </summary>
        /// <param name="obj"></param>
        private void ModifyButtonExecuteMethod(object obj)
        {
            if (SeenMember.MemberName != "")
            {
                ModifyBoxVisibility = !ModifyBoxVisibility;
                var clone = Clone();
                OriginalDataMember = (Group.Member)clone;
                IsCheckModifying = true;
            }
            else
            {
                MessageBox.Show("수정할 수 없습니다.", "Warning!", MessageBoxButton.OK);
            }
        }
        private bool ModifyButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 수정하기 완료 버튼 클릭시 실행되는 Command
        /// - 입력된 정보의 유효성 확인
        /// - SeenMember 의 입력된 정보를 선택된 Member 객체에 넣어 수정함
        /// </summary>
        /// <param name="obj"></param>
        private void ModifyCompleteButtonExecuteMethod(object obj)
        {
            if (ValidationCheck(SeenMember))
            {
                if (MessageBox.Show("수정하시겠습니까?", "수정", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (BirthToInt(OriginalDataMember.MemberBirth) == BirthToInt(SeenMember.MemberBirth))
                    {
                        ReplaceSameGroup();
                    }
                    else
                    {
                        ReplaceOtherGroup();
                    }
                    ModifyBoxVisibility = !ModifyBoxVisibility;
                    IsCheckModifying = false;
                }
            }
        }
        private bool ModifyCompleteButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 수정하기 취소 버튼 클릭시 실행되는 Command
        /// - CopyMember 내용으로 돌아가기
        /// </summary>
        /// <param name="obj"></param>
        private void ModifyCancelButtonExecuteMethod(object obj)
        {
            if (MessageBox.Show("수정을 취소하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SeenMember = OriginalDataMember;
                ModifyBoxVisibility = !ModifyBoxVisibility;
                IsCheckModifying = false;
            }
        }
        private bool ModifyCancelButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 삭제하기 버튼 클릭시 실행되는 Command
        /// - 선택한 멤버 삭제하기
        /// - 삭제후 Show Contents 부분 표시하기
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveButtonExecuteMethod(object obj)
        {
            if (SeenMember.MemberName == "")
            {
                MessageBox.Show("삭제하실 수 없습니다!", "실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (MessageBox.Show("삭제하시겠습니까?", "경고!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    RemoveMember();
                }
            }
        }
        private bool RemoveButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 등록하기 버튼 클릭시 실행되는 Command
        /// </summary>
        /// <param name="obj"></param>
        private void RegisterButtonExecuteMethod(object obj)
        {
            if (ValidationCheck(NewMember))
            {
                if (MessageBox.Show("등록하시겠습니까?", "회원등록", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    RegisterMember();
                    ChangePageVisibility = true;
                }
            }
        }
        private bool RegisterButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 취소하기 버튼 클릭시 실행되는 Command
        /// </summary>
        /// <param name="obj"></param>
        private void CancelButtonExecuteMethod(object obj)
        {
            if (MessageBox.Show("작성을 취소하시겠습니까?", "취소", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ChangePageVisibility = true;
            }
        }
        private bool CancelButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// 초기화 버튼 클륵시 실행되는 Command
        /// </summary>
        /// <param name="obj"></param>
        private void ResetButtonExecuteMethod(object obj)
        {
            if (MessageBox.Show("다시 작성 하시겠습니까?", "초기화", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ResetContents(NewMember);
            }
        }
        private bool ResetButtonCanExecuteMethod(object arg)
        {
            return true;
        }

        /// <summary>
        /// Group 안의 Member 내용들을 초기화 시켜주는 함수
        /// </summary>
        /// <param name="ResetMember"></param>
        private void ResetContents(Group.Member ResetMember)
        {
            ResetMember.MemberName = "";
            ResetMember.MemberGender = "";
            ResetMember.MemberMan = false;
            ResetMember.MemberWoman = false;
            ResetMember.MemberNothing = false;
            ResetMember.MemberBirth = "";
            ResetMember.MemberShowBirth = "";
            ResetMember.MemberAge = "";
            ResetMember.MemberPhone = "";
            ResetMember.MemberAddress = "";
            ResetMember.MemberElse = "";
            ResetMember.MemberNull = true;
        }

        /// <summary>
        /// Member Class 안의 MemberSelected 부분을 false 로 초기화 시키는 함수
        /// </summary>
        private void ResetMemberSelected()
        {
            for (var i = 0; i < UsersGroup.Count; i++)
            {
                for (var j = 0; j < UsersGroup[i].UsersMember.Count; j++)
                {
                    UsersGroup[i].UsersMember[j].MemberSelected = false;
                }
            }
        }

        /// <summary>
        /// Nav Sub Button 클릭시 Contents 부분에 정보를 보여주기 위한 함수
        /// </summary>
        /// <param name="obj"></param>
        private void ShowContents(object obj)
        {
            var member = obj as Group.Member;
            SeenMember.MemberName = member.MemberName;
            SeenMember.MemberAge = member.MemberAge;
            SeenMember.MemberBirth = member.MemberBirth;
            SeenMember.MemberShowBirth = member.MemberShowBirth;
            SeenMember.MemberAddress = member.MemberAddress;
            SeenMember.MemberElse = member.MemberElse;
            SeenMember.MemberPhone = member.MemberPhone;
            SeenMember.MemberGender = member.MemberGender;
            SeenMember.MemberMan = member.MemberMan;
            SeenMember.MemberWoman = member.MemberWoman;
            SeenMember.MemberNothing = member.MemberNothing;
            member.MemberSelected = true;
        }

        /// <summary>
        /// 수정하기 전으로 되돌아가기 위해 원래 Member 데이터를 Deep Copy 하는 함수
        /// </summary>
        /// <returns></returns>
        private object Clone()
        {
            Group.Member member = new Group.Member();
            member.MemberName = this.SeenMember.MemberName;
            member.MemberMan = this.SeenMember.MemberMan;
            member.MemberWoman = this.SeenMember.MemberWoman;
            member.MemberNothing = this.SeenMember.MemberNothing;
            member.MemberGender = this.SeenMember.MemberGender;
            member.MemberBirth = this.SeenMember.MemberBirth;
            member.MemberShowBirth = this.SeenMember.MemberShowBirth;
            member.MemberAge = this.SeenMember.MemberAge;
            member.MemberPhone = this.SeenMember.MemberPhone;
            member.MemberAddress = this.SeenMember.MemberAddress;
            member.MemberElse = this.SeenMember.MemberElse;
            member.MemberClassify = this.SeenMember.MemberClassify;

            return member;
        }

        /// <summary>
        /// 입력 데이터의 유효성 검사를 위한 함수
        /// </summary>
        /// <returns></returns>
        private bool ValidationCheck(Group.Member member)
        {
            try
            {
                bool IsNameChecker = Regex.IsMatch(member.MemberName, @"[가-힣]{2,5}");
                bool IsBirthNumberChecker = Regex.IsMatch(member.MemberBirth, @"/^[0-9]+/g");
                bool IsPhoneNumberChecker = Regex.IsMatch(member.MemberPhone, @"[0-9]{2,3}-[0-9]{3,4}-[0-9]{4}");

                if (!IsNameChecker)
                {
                    MessageBox.Show("이름을 2~5글자의 한글로 입력하세요 \n예시)김철수", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (member.MemberMan == false && member.MemberWoman == false && member.MemberNothing == false)
                {
                    MessageBox.Show("성별을 선택하세요", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (!IsPhoneNumberChecker)
                {
                    MessageBox.Show("번호를 올바르게 입력하세요 \n예시)010-1111-1111", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (member.MemberAddress == "")
                {
                    MessageBox.Show("거주지를 입력하세요", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (!IsBirthNumberChecker && member.MemberBirth.Length < 8)
                {
                    MessageBox.Show("생년월일을 8자리 숫자로 입력하세요 \n예시)19990101", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 생년월일을 나이로 바꿔주는 함수
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        private int BirthToAge(string birth)
        {
            try
            {
                return 2022 - Int32.Parse(birth.Substring(0, 4));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 19970326 -> 1997년 03월 26일 형태로 바꿔주는 함수
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        private string BirthToString(string birth)
        {
            try
            {
                return birth.Substring(0, 4) + "년 " + birth.Substring(4, 2) + "월 " + birth.Substring(6, 2) + "일";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 생년월일을 나이대 그룹으로 바꾸는 함수
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        private int BirthToInt(string birth)
        {
            if (BirthToAge(birth) < 20)
            {
                return 1;
            }
            else if (BirthToAge(birth) < 30)
            {
                return 2;
            }
            else if (BirthToAge(birth) < 40)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        /// <summary>
        /// 정보 수정시 그룹이 바뀌지 않을 경우
        /// </summary>
        private void ReplaceSameGroup()
        {
            if (BirthToInt(OriginalDataMember.MemberBirth) == 1)
            {
                ReplaceMember(MembersGroupUnderTen);
            }
            else if (BirthToInt(OriginalDataMember.MemberBirth) == 2)
            {
                ReplaceMember(MembersGroupTwenty);
            }
            else if (BirthToInt(OriginalDataMember.MemberBirth) == 3)
            {
                ReplaceMember(MembersGroupThirty);
            }
            else
            {
                ReplaceMember(MembersGroupOverFourty);
            }
        }

        /// <summary>
        /// 수정된 정보를 갈아끼워 넣는 함수(그룹이 바뀌지 않는 경우)
        /// </summary>
        /// <param name="group"></param>
        private void ReplaceMember(ObservableCollection<Group.Member> group)
        {
            try
            {
                for (var i = 0; i < group.Count; i++)
                {
                    if (group[i].MemberSelected)
                    {
                        SetGender(SeenMember);
                        group.Remove(group[i]);
                        group.Insert(i, new Group.Member() { MemberName = SeenMember.MemberName, MemberMan = SeenMember.MemberMan, MemberWoman = SeenMember.MemberWoman, MemberNothing = SeenMember.MemberNothing, MemberGender = SeenMember.MemberGender, MemberBirth = SeenMember.MemberBirth, MemberShowBirth = BirthToString(SeenMember.MemberBirth), MemberAge = BirthToAge(SeenMember.MemberBirth).ToString() + "살", MemberPhone = SeenMember.MemberPhone, MemberAddress = SeenMember.MemberAddress, MemberElse = SeenMember.MemberElse, MemberSelected = true });
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// RadioButton 에서 선택된 결과에 따른 출력 Gender 데이터를 결정하는 함수
        /// </summary>
        /// <param name="member"></param>
        private void SetGender(Group.Member member)
        {
            if (member.MemberMan)
            {
                member.MemberGender = "남자";
            }
            else if (member.MemberWoman)
            {
                member.MemberGender = "여자";
            }
            else
            {
                member.MemberGender = "선택안함";
            }
        }

        /// <summary>
        /// 정보 수정시 그룹이 바뀔 경우
        /// </summary>
        private void ReplaceOtherGroup()
        {
            if (BirthToInt(OriginalDataMember.MemberBirth) == 1)
            {
                ReplaceOtherMember(MembersGroupUnderTen, 0);
            }
            else if (BirthToInt(OriginalDataMember.MemberBirth) == 2)
            {
                ReplaceOtherMember(MembersGroupTwenty, 1);
            }
            else if (BirthToInt(OriginalDataMember.MemberBirth) == 3)
            {
                ReplaceOtherMember(MembersGroupThirty, 2);
            }
            else
            {
                ReplaceOtherMember(MembersGroupOverFourty, 3);
            }
        }

        /// <summary>
        /// 수정된 정보를 갈아끼워 넣는 함수(그룹이 바뀌는 경우)
        /// </summary>
        /// <param name="member"></param>
        /// <param name="index"></param>
        private void ReplaceOtherMember(ObservableCollection<Group.Member> member, int index)
        {
            for (var i = 0; i < member.Count; i++)
            {
                if (member[i].MemberSelected)
                {
                    member.Remove(member[i]);
                    AddOtherMember();
                    if (member.Count == 0)
                    {
                        ResetContents(SeenMember);
                    }
                    else
                    {
                        AfterRemoveShowContents(member);
                    }
                    AddOtherGroup(member, index);
                }
            }
        }

        /// <summary>
        /// 수정된 정보를 바뀐 그룹에 추가하는 과정
        /// </summary>
        private void AddOtherMember()
        {
            SetGender(SeenMember);
            if (BirthToInt(SeenMember.MemberBirth) == 1)
            {
                MembersGroupUnderTen.Add(new Group.Member() { MemberName = SeenMember.MemberName, MemberMan = SeenMember.MemberMan, MemberWoman = SeenMember.MemberWoman, MemberNothing = SeenMember.MemberNothing, MemberGender = SeenMember.MemberGender, MemberBirth = SeenMember.MemberBirth, MemberShowBirth = BirthToString(SeenMember.MemberBirth), MemberAge = BirthToAge(SeenMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(SeenMember.MemberBirth), MemberPhone = SeenMember.MemberPhone, MemberAddress = SeenMember.MemberAddress, MemberElse = SeenMember.MemberElse });
                UsersGroup[0] = new Group { UserGroupTitle = "10대이하", UsersMember = MembersGroupUnderTen, PeopleCount = MembersGroupUnderTen.Count };
            }
            else if (BirthToInt(SeenMember.MemberBirth) == 2)
            {
                MembersGroupTwenty.Add(new Group.Member() { MemberName = SeenMember.MemberName, MemberMan = SeenMember.MemberMan, MemberWoman = SeenMember.MemberWoman, MemberNothing = SeenMember.MemberNothing, MemberGender = SeenMember.MemberGender, MemberBirth = SeenMember.MemberBirth, MemberShowBirth = BirthToString(SeenMember.MemberBirth), MemberAge = BirthToAge(SeenMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(SeenMember.MemberBirth), MemberPhone = SeenMember.MemberPhone, MemberAddress = SeenMember.MemberAddress, MemberElse = SeenMember.MemberElse });
                UsersGroup[1] = new Group { UserGroupTitle = "20대", UsersMember = MembersGroupTwenty, PeopleCount = MembersGroupTwenty.Count };
            }
            else if (BirthToInt(SeenMember.MemberBirth) == 3)
            {
                MembersGroupThirty.Add(new Group.Member() { MemberName = SeenMember.MemberName, MemberMan = SeenMember.MemberMan, MemberWoman = SeenMember.MemberWoman, MemberNothing = SeenMember.MemberNothing, MemberGender = SeenMember.MemberGender, MemberBirth = SeenMember.MemberBirth, MemberShowBirth = BirthToString(SeenMember.MemberBirth), MemberAge = BirthToAge(SeenMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(SeenMember.MemberBirth), MemberPhone = SeenMember.MemberPhone, MemberAddress = SeenMember.MemberAddress, MemberElse = SeenMember.MemberElse });
                UsersGroup[2] = new Group { UserGroupTitle = "30대", UsersMember = MembersGroupThirty, PeopleCount = MembersGroupThirty.Count };
            }
            else
            {
                MembersGroupOverFourty.Add(new Group.Member() { MemberName = SeenMember.MemberName, MemberMan = SeenMember.MemberMan, MemberWoman = SeenMember.MemberWoman, MemberNothing = SeenMember.MemberNothing, MemberGender = SeenMember.MemberGender, MemberBirth = SeenMember.MemberBirth, MemberShowBirth = BirthToString(SeenMember.MemberBirth), MemberAge = BirthToAge(SeenMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(SeenMember.MemberBirth), MemberPhone = SeenMember.MemberPhone, MemberAddress = SeenMember.MemberAddress, MemberElse = SeenMember.MemberElse });
                UsersGroup[3] = new Group { UserGroupTitle = "40대이상", UsersMember = MembersGroupOverFourty, PeopleCount = MembersGroupOverFourty.Count };
            }
        }

        /// <summary>
        /// 보여지는 부분에 Member 내용들을 넣어주는 함수
        /// </summary>
        /// <param name="memberAge"></param>
        private void AfterRemoveShowContents(ObservableCollection<Group.Member> member)
        {
            if (member.Count != 0)
            {
                member[0].MemberSelected = true;
                SeenMember.MemberName = member[0].MemberName;
                SeenMember.MemberGender = member[0].MemberGender;
                SeenMember.MemberMan = member[0].MemberMan;
                SeenMember.MemberWoman = member[0].MemberWoman;
                SeenMember.MemberNothing = member[0].MemberNothing;
                SeenMember.MemberAge = BirthToAge(member[0].MemberBirth).ToString() + "살";
                SeenMember.MemberPhone = member[0].MemberPhone;
                SeenMember.MemberBirth = member[0].MemberBirth;
                SeenMember.MemberShowBirth = BirthToString(member[0].MemberBirth);
                SeenMember.MemberAddress = member[0].MemberAddress;
                SeenMember.MemberElse = member[0].MemberElse;
            }
            else
            {
                MessageBox.Show("회원등록을 먼저 진행하세요!");
            }
        }

        /// <summary>
        /// 수정된 Member 를 포함한 Group 으로 교체하는 함수
        /// </summary>
        /// <param name="member"></param>
        /// <param name="index"></param>
        private void AddOtherGroup(ObservableCollection<Group.Member> member, int index)
        {
            if (index == 0)
            {
                UsersGroup[index] = new Group { UserGroupTitle = "10대이하", UsersMember = member, PeopleCount = member.Count };
            }
            else if (index == 1)
            {
                UsersGroup[index] = new Group { UserGroupTitle = "20대", UsersMember = member, PeopleCount = member.Count };
            }
            else if (index == 2)
            {
                UsersGroup[index] = new Group { UserGroupTitle = "30대", UsersMember = member, PeopleCount = member.Count };
            }
            else
            {
                UsersGroup[index] = new Group { UserGroupTitle = "40대이상", UsersMember = member, PeopleCount = member.Count };
            }
        }

        /// <summary>
        /// 각 Group 에서 선택된 Member를 삭제한다.
        /// </summary>
        private void RemoveMember()
        {
            if (BirthToInt(SeenMember.MemberBirth) == 1)
            {
                RemoveSetting(MembersGroupUnderTen, 0);
            }
            else if (BirthToInt(SeenMember.MemberBirth) == 2)
            {
                RemoveSetting(MembersGroupTwenty, 1);
            }
            else if (BirthToInt(SeenMember.MemberBirth) == 3)
            {
                RemoveSetting(MembersGroupThirty, 2);
            }
            else
            {
                RemoveSetting(MembersGroupOverFourty, 3);
            }
        }

        /// <summary>
        /// Member를 지우고 Show Contents 부분을 세팅한다.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="index"></param>
        private void RemoveSetting(ObservableCollection<Group.Member> member, int index)
        {
            for (var i = 0; i < member.Count; i++)
            {
                if (member[i].MemberName == SeenMember.MemberName)
                {
                    member.Remove(member[i]);
                    // 첫 화면 default 값(임의)
                    if (member.Count == 0)
                    {
                        ResetContents(SeenMember);
                    }
                    else
                    {
                        AfterRemoveShowContents(member);
                    }
                    AddOtherGroup(member, index);
                }
            }
        }

        /// <summary>
        /// NewMember 의 데이터를 통해 Group 을 선택하고 Member 로 추가한다.
        /// </summary>
        private void RegisterMember()
        {
            SetGender(NewMember);
            if (BirthToInt(NewMember.MemberBirth) == 1)
            {
                MembersGroupUnderTen.Add(new Group.Member() { MemberName = NewMember.MemberName, MemberMan = NewMember.MemberMan, MemberWoman = NewMember.MemberWoman, MemberNothing = NewMember.MemberNothing, MemberGender = NewMember.MemberGender, MemberBirth = NewMember.MemberBirth, MemberShowBirth = BirthToString(NewMember.MemberBirth), MemberAge = BirthToAge(NewMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(NewMember.MemberBirth), MemberPhone = NewMember.MemberPhone, MemberAddress = NewMember.MemberAddress, MemberElse = NewMember.MemberElse });
                UsersGroup[0] = new Group { UserGroupTitle = "10대이하", UsersMember = MembersGroupUnderTen, PeopleCount = MembersGroupUnderTen.Count };
            }
            else if (BirthToInt(NewMember.MemberBirth) == 2)
            {
                MembersGroupTwenty.Add(new Group.Member() { MemberName = NewMember.MemberName, MemberMan = NewMember.MemberMan, MemberWoman = NewMember.MemberWoman, MemberNothing = NewMember.MemberNothing, MemberGender = NewMember.MemberGender, MemberBirth = NewMember.MemberBirth, MemberShowBirth = BirthToString(NewMember.MemberBirth), MemberAge = BirthToAge(NewMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(NewMember.MemberBirth), MemberPhone = NewMember.MemberPhone, MemberAddress = NewMember.MemberAddress, MemberElse = NewMember.MemberElse });
                UsersGroup[1] = new Group { UserGroupTitle = "20대", UsersMember = MembersGroupTwenty, PeopleCount = MembersGroupTwenty.Count };
            }
            else if (BirthToInt(NewMember.MemberBirth) == 3)
            {
                MembersGroupThirty.Add(new Group.Member() { MemberName = NewMember.MemberName, MemberMan = NewMember.MemberMan, MemberWoman = NewMember.MemberWoman, MemberNothing = NewMember.MemberNothing, MemberGender = NewMember.MemberGender, MemberBirth = NewMember.MemberBirth, MemberShowBirth = BirthToString(NewMember.MemberBirth), MemberAge = BirthToAge(NewMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(NewMember.MemberBirth), MemberPhone = NewMember.MemberPhone, MemberAddress = NewMember.MemberAddress, MemberElse = NewMember.MemberElse });
                UsersGroup[2] = new Group { UserGroupTitle = "30대", UsersMember = MembersGroupThirty, PeopleCount = MembersGroupThirty.Count };
            }
            else
            {
                MembersGroupOverFourty.Add(new Group.Member() { MemberName = NewMember.MemberName, MemberMan = NewMember.MemberMan, MemberWoman = NewMember.MemberWoman, MemberNothing = NewMember.MemberNothing, MemberGender = NewMember.MemberGender, MemberBirth = NewMember.MemberBirth, MemberShowBirth = BirthToString(NewMember.MemberBirth), MemberAge = BirthToAge(NewMember.MemberBirth).ToString() + "살", MemberClassify = BirthToInt(NewMember.MemberBirth), MemberPhone = NewMember.MemberPhone, MemberAddress = NewMember.MemberAddress, MemberElse = NewMember.MemberElse });
                UsersGroup[3] = new Group { UserGroupTitle = "40대이상", UsersMember = MembersGroupOverFourty, PeopleCount = MembersGroupOverFourty.Count };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Visibility 값을 바꿔주는 Converter
    /// </summary>
    public class ReverseVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Bool 값을 Color 값으로 바꿔주는 Converter
    /// </summary>
    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
