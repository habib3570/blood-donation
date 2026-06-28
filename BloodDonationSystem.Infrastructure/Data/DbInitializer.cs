using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "Donor", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin
            if (!await userManager.Users.AnyAsync(x => x.Email == "admin@blooddonation.com"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@blooddonation.com",
                    Email = "admin@blooddonation.com",
                    FullName = "System Admin",
                    BloodGroup = BloodGroup.OPositive,
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    District = "Dhaka",
                    Upazila = "Mirpur",
                    IsVerified = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Achievements
            if (!await context.Achievements.AnyAsync())
            {
                var achievements = new List<Achievement>
                {
                    new() { Title = "First Donation", Description = "Completed your first blood donation", Type = AchievementType.FirstDonation, RequiredCount = 1, RewardPoints = 100 },
                    new() { Title = "5 Donations Club", Description = "Donated blood 5 times", Type = AchievementType.FiveDonationsClub, RequiredCount = 5, RewardPoints = 250 },
                    new() { Title = "10 Donations Club", Description = "Donated blood 10 times", Type = AchievementType.TenDonationsClub, RequiredCount = 10, RewardPoints = 500 },
                    new() { Title = "Emergency Hero", Description = "Responded to 3 emergency requests", Type = AchievementType.EmergencyHero, RequiredCount = 3, RewardPoints = 300 },
                    new() { Title = "10 Lives Saved", Description = "Estimated 10 lives saved", Type = AchievementType.TenLivesSaved, RequiredCount = 10, RewardPoints = 400 }
                };
                await context.Achievements.AddRangeAsync(achievements);
            }

            // Seed Badges
            if (!await context.Badges.AnyAsync())
            {
                var badges = new List<Badge>
                {
                    new() { Name = "Bronze Donor", Description = "Donated blood 1-4 times", Type = BadgeType.Bronze, RequiredDonations = 1 },
                    new() { Name = "Silver Donor", Description = "Donated blood 5-9 times", Type = BadgeType.Silver, RequiredDonations = 5 },
                    new() { Name = "Gold Donor", Description = "Donated blood 10-19 times", Type = BadgeType.Gold, RequiredDonations = 10 },
                    new() { Name = "Platinum Donor", Description = "Donated blood 20+ times", Type = BadgeType.Platinum, RequiredDonations = 20 },
                    new() { Name = "Emergency Hero", Description = "Responded to emergency requests", Type = BadgeType.EmergencyHero, RequiredDonations = 0 }
                };
                await context.Badges.AddRangeAsync(badges);
            }

            // Seed FAQs
            if (!await context.FAQs.AnyAsync())
            {
                var faqs = new List<FAQ>
                {
                    new() { Question = "How old do you need to be to donate blood?", Answer = "You must be at least 18 years old to donate blood.", QuestionBn = "রক্ত দিতে কত বয়স লাগে?", AnswerBn = "রক্ত দিতে কমপক্ষে ১৮ বছর বয়স হতে হবে।", DisplayOrder = 1 },
                    new() { Question = "How often can you donate blood?", Answer = "You can donate whole blood every 90 days (3 months).", QuestionBn = "কতদিন পর আবার রক্ত দেওয়া যায়?", AnswerBn = "প্রতি ৯০ দিন (৩ মাস) পর পর রক্ত দেওয়া যায়।", DisplayOrder = 2 },
                    new() { Question = "What is the minimum weight to donate blood?", Answer = "You must weigh at least 50 kg to donate blood.", QuestionBn = "রক্ত দিতে ন্যূনতম ওজন কত?", AnswerBn = "রক্ত দিতে কমপক্ষে ৫০ কেজি ওজন হতে হবে।", DisplayOrder = 3 },
                    new() { Question = "How long does blood donation take?", Answer = "The actual donation takes about 8-10 minutes. The whole process takes about 45-60 minutes.", QuestionBn = "রক্তদানে কতক্ষণ সময় লাগে?", AnswerBn = "মূল রক্তদান প্রায় ৮-১০ মিনিট সময় নেয়। পুরো প্রক্রিয়াটি প্রায় ৪৫-৬০ মিনিট নেয়।", DisplayOrder = 4 }
                };
                await context.FAQs.AddRangeAsync(faqs);
            }

            await context.SaveChangesAsync();

            // Seed Health Tips
            if (!await context.HealthTips.AnyAsync())
            {
                var tips = new List<HealthTip>
                {
                    new() { Title = "Before Donation", Content = "Drink plenty of water and eat a healthy meal before donating.", TitleBn = "দান করার আগে", ContentBn = "রক্ত দেওয়ার আগে প্রচুর পানি পান করুন এবং স্বাস্থ্যকর খাবার খান।", Category = "Before" },
                    new() { Title = "After Donation", Content = "Rest for at least 10-15 minutes after donating. Avoid heavy exercise.", TitleBn = "দানের পরে", ContentBn = "রক্ত দেওয়ার পরে কমপক্ষে ১০-১৫ মিনিট বিশ্রাম নিন। ভারী ব্যায়াম এড়িয়ে চলুন।", Category = "After" },
                    new() { Title = "Who Can Donate", Content = "Anyone aged 18-60, weighing at least 50 kg, and in good health can donate blood.", TitleBn = "কে রক্ত দিতে পারবে", ContentBn = "১৮-৬০ বছর বয়সী, কমপক্ষে ৫০ কেজি ওজনের এবং সুস্বাস্থ্যের অধিকারী যে কেউ রক্ত দিতে পারবেন।", Category = "Eligibility" }
                };
                await context.HealthTips.AddRangeAsync(tips);
            }

            await context.SaveChangesAsync();

            // Seed Districts and Upazilas (আলাদা মেথড, একই ক্লাসে, SeedAsync এর বাইরে)
            await SeedDistrictsAndUpazilasAsync(context);
        }

        public static async Task SeedDistrictsAndUpazilasAsync(ApplicationDbContext context)
        {
            if (await context.Districts.AnyAsync()) return;

            var data = new Dictionary<string, (string Division, string[] Upazilas)>
            {
                ["Bagerhat"] = ("Khulna", new[] { "Bagerhat Sadar", "Chitalmari", "Fakirhat", "Kachua", "Mollahat", "Mongla", "Morrelganj", "Rampal", "Sarankhola" }),
                ["Bandarban"] = ("Chittagong", new[] { "Bandarban Sadar", "Alikadam", "Lama", "Naikhongchhari", "Rowangchhari", "Ruma", "Thanchi" }),
                ["Barguna"] = ("Barisal", new[] { "Amtali", "Bamna", "Barguna Sadar", "Betagi", "Patharghata", "Taltali" }),
                ["Barisal"] = ("Barisal", new[] { "Agailjhara", "Babuganj", "Bakerganj", "Banaripara", "Barisal Sadar", "Gaurnadi", "Hizla", "Mehendiganj", "Muladi", "Wazirpur" }),
                ["Bhola"] = ("Barisal", new[] { "Bhola Sadar", "Borhanuddin", "Char Fasson", "Daulatkhan", "Lalmohan", "Manpura", "Tazumuddin" }),
                ["Bogura"] = ("Rajshahi", new[] { "Adamdighi", "Bogura Sadar", "Dhunat", "Dupchanchia", "Gabtali", "Kahaloo", "Nandigram", "Sariakandi", "Shajahanpur", "Sherpur", "Shibganj", "Sonatala" }),
                ["Brahmanbaria"] = ("Chittagong", new[] { "Akhaura", "Bancharampur", "Bijoynagar", "Brahmanbaria Sadar", "Kasba", "Nabinagar", "Nasirnagar", "Sarail", "Ashuganj" }),
                ["Chandpur"] = ("Chittagong", new[] { "Chandpur Sadar", "Faridganj", "Haimchar", "Haziganj", "Kachua", "Matlab Dakshin", "Matlab Uttar", "Shahrasti" }),
                ["Chittagong"] = ("Chittagong", new[] { "Anwara", "Banshkhali", "Boalkhali", "Chandanaish", "Fatikchhari", "Hathazari", "Lohagara", "Mirsharai", "Patiya", "Rangunia", "Raozan", "Sandwip", "Satkania", "Sitakunda" }),
                ["Chuadanga"] = ("Khulna", new[] { "Alamdanga", "Chuadanga Sadar", "Damurhuda", "Jibannagar" }),
                ["Comilla"] = ("Chittagong", new[] { "Barura", "Brahmanpara", "Burichang", "Chandina", "Chauddagram", "Comilla Sadar", "Daudkandi", "Debidwar", "Homna", "Laksam", "Lalmai", "Meghna", "Monohorgonj", "Muradnagar", "Nangalkot", "Titas" }),
                ["Cox's Bazar"] = ("Chittagong", new[] { "Chakaria", "Cox's Bazar Sadar", "Kutubdia", "Maheshkhali", "Pekua", "Ramu", "Teknaf", "Ukhia" }),
                ["Dhaka"] = ("Dhaka", new[] { "Dhamrai", "Dohar", "Keraniganj", "Nawabganj", "Savar" }),
                ["Dinajpur"] = ("Rangpur", new[] { "Birampur", "Birganj", "Biral", "Bochaganj", "Chirirbandar", "Dinajpur Sadar", "Ghoraghat", "Hakimpur", "Kaharole", "Khansama", "Nawabganj", "Parbatipur" }),
                ["Faridpur"] = ("Dhaka", new[] { "Alfadanga", "Bhanga", "Boalmari", "Charbhadrasan", "Faridpur Sadar", "Madhukhali", "Nagarkanda", "Sadarpur", "Saltha" }),
                ["Feni"] = ("Chittagong", new[] { "Chhagalnaiya", "Daganbhuiyan", "Feni Sadar", "Fulgazi", "Parshuram", "Sonagazi" }),
                ["Gaibandha"] = ("Rangpur", new[] { "Fulchhari", "Gaibandha Sadar", "Gobindaganj", "Palashbari", "Sadullapur", "Saghata", "Sundarganj" }),
                ["Gazipur"] = ("Dhaka", new[] { "Gazipur Sadar", "Kaliakair", "Kaliganj", "Kapasia", "Sreepur" }),
                ["Gopalganj"] = ("Dhaka", new[] { "Gopalganj Sadar", "Kashiani", "Kotalipara", "Muksudpur", "Tungipara" }),
                ["Habiganj"] = ("Sylhet", new[] { "Ajmiriganj", "Bahubal", "Baniyachong", "Chunarughat", "Habiganj Sadar", "Lakhai", "Madhabpur", "Nabiganj", "Shayestaganj" }),
                ["Jamalpur"] = ("Mymensingh", new[] { "Bakshiganj", "Dewanganj", "Islampur", "Jamalpur Sadar", "Madarganj", "Melandaha", "Sarishabari" }),
                ["Jessore"] = ("Khulna", new[] { "Abhaynagar", "Bagherpara", "Chaugachha", "Jessore Sadar", "Jhikargachha", "Keshabpur", "Manirampur", "Sharsha" }),
                ["Jhalokati"] = ("Barisal", new[] { "Jhalokati Sadar", "Kathalia", "Nalchity", "Rajapur" }),
                ["Jhenaidah"] = ("Khulna", new[] { "Harinakunda", "Jhenaidah Sadar", "Kaliganj", "Kotchandpur", "Maheshpur", "Shailkupa" }),
                ["Joypurhat"] = ("Rajshahi", new[] { "Akkelpur", "Joypurhat Sadar", "Kalai", "Khetlal", "Panchbibi" }),
                ["Khagrachhari"] = ("Chittagong", new[] { "Dighinala", "Khagrachhari Sadar", "Lakshmichhari", "Mahalchhari", "Manikchhari", "Matiranga", "Panchhari", "Ramgarh" }),
                ["Khulna"] = ("Khulna", new[] { "Batiaghata", "Dacope", "Dumuria", "Dighalia", "Koyra", "Paikgachha", "Phultala", "Rupsa", "Terokhada" }),
                ["Kishoreganj"] = ("Dhaka", new[] { "Austagram", "Bajitpur", "Bhairab", "Hossainpur", "Itna", "Karimganj", "Katiadi", "Kishoreganj Sadar", "Kuliarchar", "Mithamain", "Nikli", "Pakundia", "Tarail" }),
                ["Kurigram"] = ("Rangpur", new[] { "Bhurungamari", "Char Rajibpur", "Chilmari", "Kurigram Sadar", "Nageshwari", "Phulbari", "Rajarhat", "Raomari", "Ulipur" }),
                ["Kushtia"] = ("Khulna", new[] { "Bheramara", "Daulatpur", "Khoksa", "Kumarkhali", "Kushtia Sadar", "Mirpur" }),
                ["Lakshmipur"] = ("Chittagong", new[] { "Lakshmipur Sadar", "Raipur", "Ramganj", "Ramgati", "Kamalnagar" }),
                ["Lalmonirhat"] = ("Rangpur", new[] { "Aditmari", "Hatibandha", "Kaliganj", "Lalmonirhat Sadar", "Patgram" }),
                ["Madaripur"] = ("Dhaka", new[] { "Kalkini", "Madaripur Sadar", "Rajoir", "Shibchar" }),
                ["Magura"] = ("Khulna", new[] { "Magura Sadar", "Mohammadpur", "Shalikha", "Sreepur" }),
                ["Manikganj"] = ("Dhaka", new[] { "Daulatpur", "Ghior", "Harirampur", "Manikganj Sadar", "Saturia", "Shivalaya", "Singair" }),
                ["Meherpur"] = ("Khulna", new[] { "Gangni", "Meherpur Sadar", "Mujibnagar" }),
                ["Moulvibazar"] = ("Sylhet", new[] { "Barlekha", "Kamalganj", "Kulaura", "Moulvibazar Sadar", "Rajnagar", "Sreemangal", "Juri" }),
                ["Munshiganj"] = ("Dhaka", new[] { "Gazaria", "Lauhajang", "Munshiganj Sadar", "Sirajdikhan", "Sreenagar", "Tongibari" }),
                ["Mymensingh"] = ("Mymensingh", new[] { "Bhaluka", "Dhobaura", "Fulbaria", "Gaffargaon", "Gauripur", "Haluaghat", "Ishwarganj", "Mymensingh Sadar", "Nandail", "Phulpur", "Tarakanda", "Trishal" }),
                ["Naogaon"] = ("Rajshahi", new[] { "Atrai", "Badalgachhi", "Dhamoirhat", "Manda", "Mahadebpur", "Naogaon Sadar", "Niamatpur", "Patnitala", "Porsha", "Raninagar", "Sapahar" }),
                ["Narail"] = ("Khulna", new[] { "Kalia", "Lohagara", "Narail Sadar" }),
                ["Narayanganj"] = ("Dhaka", new[] { "Araihazar", "Bandar", "Narayanganj Sadar", "Rupganj", "Sonargaon" }),
                ["Narsingdi"] = ("Dhaka", new[] { "Belabo", "Monohardi", "Narsingdi Sadar", "Palash", "Raipura", "Shibpur" }),
                ["Natore"] = ("Rajshahi", new[] { "Bagatipara", "Baraigram", "Gurudaspur", "Lalpur", "Natore Sadar", "Singra" }),
                ["Netrokona"] = ("Mymensingh", new[] { "Atpara", "Barhatta", "Durgapur", "Khaliajuri", "Kalmakanda", "Kendua", "Madan", "Mohanganj", "Netrokona Sadar", "Purbadhala" }),
                ["Nilphamari"] = ("Rangpur", new[] { "Dimla", "Domar", "Jaldhaka", "Kishoreganj", "Nilphamari Sadar", "Saidpur" }),
                ["Noakhali"] = ("Chittagong", new[] { "Begumganj", "Chatkhil", "Companiganj", "Hatiya", "Kabirhat", "Noakhali Sadar", "Senbagh", "Sonaimuri", "Subarnachar" }),
                ["Pabna"] = ("Rajshahi", new[] { "Atgharia", "Bera", "Bhangura", "Chatmohar", "Faridpur", "Ishwardi", "Pabna Sadar", "Santhia", "Sujanagar" }),
                ["Panchagarh"] = ("Rangpur", new[] { "Atwari", "Boda", "Debiganj", "Panchagarh Sadar", "Tetulia" }),
                ["Patuakhali"] = ("Barisal", new[] { "Bauphal", "Dashmina", "Dumki", "Galachipa", "Kalapara", "Mirzaganj", "Patuakhali Sadar", "Rangabali" }),
                ["Pirojpur"] = ("Barisal", new[] { "Bhandaria", "Kawkhali", "Mathbaria", "Nazirpur", "Nesarabad", "Pirojpur Sadar", "Zianagar" }),
                ["Rajbari"] = ("Dhaka", new[] { "Baliakandi", "Goalandaghat", "Pangsha", "Rajbari Sadar", "Kalukhali" }),
                ["Rajshahi"] = ("Rajshahi", new[] { "Bagha", "Bagmara", "Charghat", "Durgapur", "Godagari", "Mohanpur", "Paba", "Puthia", "Tanore" }),
                ["Rangamati"] = ("Chittagong", new[] { "Baghaichhari", "Barkal", "Belaichhari", "Kaptai", "Juraichhari", "Langadu", "Nannerchar", "Rajasthali", "Rangamati Sadar" }),
                ["Rangpur"] = ("Rangpur", new[] { "Badarganj", "Gangachara", "Kaunia", "Mithapukur", "Pirgachha", "Pirganj", "Rangpur Sadar", "Taraganj" }),
                ["Satkhira"] = ("Khulna", new[] { "Assasuni", "Debhata", "Kalaroa", "Kaliganj", "Satkhira Sadar", "Shyamnagar", "Tala" }),
                ["Shariatpur"] = ("Dhaka", new[] { "Bhedarganj", "Damudya", "Gosairhat", "Naria", "Shariatpur Sadar", "Zajira" }),
                ["Sherpur"] = ("Mymensingh", new[] { "Jhenaigati", "Nakla", "Nalitabari", "Sherpur Sadar", "Sreebardi" }),
                ["Sirajganj"] = ("Rajshahi", new[] { "Belkuchi", "Chauhali", "Kamarkhanda", "Kazipur", "Raiganj", "Shahjadpur", "Sirajganj Sadar", "Tarash", "Ullapara" }),
                ["Sunamganj"] = ("Sylhet", new[] { "Bishwamvarpur", "Chhatak", "Derai", "Dharampasha", "Dowarabazar", "Jagannathpur", "Jamalganj", "Sullah", "Sunamganj Sadar", "Tahirpur" }),
                ["Sylhet"] = ("Sylhet", new[] { "Balaganj", "Beanibazar", "Bishwanath", "Companiganj", "Fenchuganj", "Golapganj", "Gowainghat", "Jaintiapur", "Kanaighat", "Osmaninagar", "Sylhet Sadar", "Zakiganj" }),
                ["Tangail"] = ("Dhaka", new[] { "Basail", "Bhuapur", "Delduar", "Dhanbari", "Ghatail", "Gopalpur", "Kalihati", "Madhupur", "Mirzapur", "Nagarpur", "Sakhipur", "Tangail Sadar" }),
                ["Thakurgaon"] = ("Rangpur", new[] { "Baliadangi", "Haripur", "Pirganj", "Ranisankail", "Thakurgaon Sadar" })
            };

            foreach (var item in data)
            {
                var district = new District
                {
                    Name = item.Key,
                    Division = item.Value.Division,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var upazilaName in item.Value.Upazilas)
                {
                    district.Upazilas.Add(new Upazila
                    {
                        Name = upazilaName,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await context.Districts.AddAsync(district);
            }

            await context.SaveChangesAsync();
        }
    }
}