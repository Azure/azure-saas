const timezonesOptions = [
  {
    value: "Azores Standard Time",
    offset: "+00:00",
    text: "Azores Standard Time (+00:00)",
  },
  {
    value: "Greenwich Standard Time",
    offset: "+00:00",
    text: "Greenwich Standard Time (+00:00)",
  },
  {
    value: "Sao Tome Standard Time",
    offset: "+00:00",
    text: "Sao Tome Standard Time (+00:00)",
  },
  {
    value: "UTC",
    offset: "+00:00",
    text: "UTC (+00:00)",
  },
  {
    value: "GMT Standard Time",
    offset: "+01:00",
    text: "GMT Standard Time (+01:00)",
  },
  {
    value: "Morocco Standard Time",
    offset: "+01:00",
    text: "Morocco Standard Time (+01:00)",
  },
  {
    value: "W. Central Africa Standard Time",
    offset: "+01:00",
    text: "W. Central Africa Standard Time (+01:00)",
  },
  {
    value: "W. Europe Standard Time",
    offset: "+02:00",
    text: "W. Europe Standard Time (+02:00)",
  },
  {
    value: "Central Europe Standard Time",
    offset: "+02:00",
    text: "Central Europe Standard Time (+02:00)",
  },
  {
    value: "Romance Standard Time",
    offset: "+02:00",
    text: "Romance Standard Time (+02:00)",
  },
  {
    value: "Central European Standard Time",
    offset: "+02:00",
    text: "Central European Standard Time (+02:00)",
  },
  {
    value: "South Africa Standard Time",
    offset: "+02:00",
    text: "South Africa Standard Time (+02:00)",
  },
  {
    value: "South Sudan Standard Time",
    offset: "+02:00",
    text: "South Sudan Standard Time (+02:00)",
  },
  {
    value: "Kaliningrad Standard Time",
    offset: "+02:00",
    text: "Kaliningrad Standard Time (+02:00)",
  },
  {
    value: "Sudan Standard Time",
    offset: "+02:00",
    text: "Sudan Standard Time (+02:00)",
  },
  {
    value: "Libya Standard Time",
    offset: "+02:00",
    text: "Libya Standard Time (+02:00)",
  },
  {
    value: "Namibia Standard Time",
    offset: "+02:00",
    text: "Namibia Standard Time (+02:00)",
  },
  {
    value: "Jordan Standard Time",
    offset: "+03:00",
    text: "Jordan Standard Time (+03:00)",
  },
  {
    value: "Arabic Standard Time",
    offset: "+03:00",
    text: "Arabic Standard Time (+03:00)",
  },
  {
    value: "Turkey Standard Time",
    offset: "+03:00",
    text: "Turkey Standard Time (+03:00)",
  },
  {
    value: "Arab Standard Time",
    offset: "+03:00",
    text: "Arab Standard Time (+03:00)",
  },
  {
    value: "Belarus Standard Time",
    offset: "+03:00",
    text: "Belarus Standard Time (+03:00)",
  },
  {
    value: "Russian Standard Time",
    offset: "+03:00",
    text: "Russian Standard Time (+03:00)",
  },
  {
    value: "E. Africa Standard Time",
    offset: "+03:00",
    text: "E. Africa Standard Time (+03:00)",
  },
  {
    value: "Volgograd Standard Time",
    offset: "+03:00",
    text: "Volgograd Standard Time (+03:00)",
  },
  {
    value: "FLE Standard Time",
    offset: "+03:00",
    text: "FLE Standard Time (+03:00)",
  },
  {
    value: "Israel Standard Time",
    offset: "+03:00",
    text: "Israel Standard Time (+03:00)",
  },
  {
    value: "GTB Standard Time",
    offset: "+03:00",
    text: "GTB Standard Time (+03:00)",
  },
  {
    value: "Middle East Standard Time",
    offset: "+03:00",
    text: "Middle East Standard Time (+03:00)",
  },
  {
    value: "Egypt Standard Time",
    offset: "+03:00",
    text: "Egypt Standard Time (+03:00)",
  },
  {
    value: "E. Europe Standard Time",
    offset: "+03:00",
    text: "E. Europe Standard Time (+03:00)",
  },
  {
    value: "Syria Standard Time",
    offset: "+03:00",
    text: "Syria Standard Time (+03:00)",
  },
  {
    value: "West Bank Standard Time",
    offset: "+03:00",
    text: "West Bank Standard Time (+03:00)",
  },
  {
    value: "Iran Standard Time",
    offset: "+03:30",
    text: "Iran Standard Time (+03:30)",
  },
  {
    value: "Arabian Standard Time",
    offset: "+04:00",
    text: "Arabian Standard Time (+04:00)",
  },
  {
    value: "Astrakhan Standard Time",
    offset: "+04:00",
    text: "Astrakhan Standard Time (+04:00)",
  },
  {
    value: "Azerbaijan Standard Time",
    offset: "+04:00",
    text: "Azerbaijan Standard Time (+04:00)",
  },
  {
    value: "Russia Time Zone 3",
    offset: "+04:00",
    text: "Russia Time Zone 3 (+04:00)",
  },
  {
    value: "Mauritius Standard Time",
    offset: "+04:00",
    text: "Mauritius Standard Time (+04:00)",
  },
  {
    value: "Saratov Standard Time",
    offset: "+04:00",
    text: "Saratov Standard Time (+04:00)",
  },
  {
    value: "Georgian Standard Time",
    offset: "+04:00",
    text: "Georgian Standard Time (+04:00)",
  },
  {
    value: "Caucasus Standard Time",
    offset: "+04:00",
    text: "Caucasus Standard Time (+04:00)",
  },
  {
    value: "Afghanistan Standard Time",
    offset: "+04:30",
    text: "Afghanistan Standard Time (+04:30)",
  },
  {
    value: "West Asia Standard Time",
    offset: "+05:00",
    text: "West Asia Standard Time (+05:00)",
  },
  {
    value: "Ekaterinburg Standard Time",
    offset: "+05:00",
    text: "Ekaterinburg Standard Time (+05:00)",
  },
  {
    value: "Pakistan Standard Time",
    offset: "+05:00",
    text: "Pakistan Standard Time (+05:00)",
  },
  {
    value: "Qyzylorda Standard Time",
    offset: "+05:00",
    text: "Qyzylorda Standard Time (+05:00)",
  },
  {
    value: "India Standard Time",
    offset: "+05:30",
    text: "India Standard Time (+05:30)",
  },
  {
    value: "Sri Lanka Standard Time",
    offset: "+05:30",
    text: "Sri Lanka Standard Time (+05:30)",
  },
  {
    value: "Nepal Standard Time",
    offset: "+05:45",
    text: "Nepal Standard Time (+05:45)",
  },
  {
    value: "Central Asia Standard Time",
    offset: "+06:00",
    text: "Central Asia Standard Time (+06:00)",
  },
  {
    value: "Bangladesh Standard Time",
    offset: "+06:00",
    text: "Bangladesh Standard Time (+06:00)",
  },
  {
    value: "Omsk Standard Time",
    offset: "+06:00",
    text: "Omsk Standard Time (+06:00)",
  },
  {
    value: "Myanmar Standard Time",
    offset: "+06:30",
    text: "Myanmar Standard Time (+06:30)",
  },
  {
    value: "SE Asia Standard Time",
    offset: "+07:00",
    text: "SE Asia Standard Time (+07:00)",
  },
  {
    value: "Altai Standard Time",
    offset: "+07:00",
    text: "Altai Standard Time (+07:00)",
  },
  {
    value: "W. Mongolia Standard Time",
    offset: "+07:00",
    text: "W. Mongolia Standard Time (+07:00)",
  },
  {
    value: "North Asia Standard Time",
    offset: "+07:00",
    text: "North Asia Standard Time (+07:00)",
  },
  {
    value: "N. Central Asia Standard Time",
    offset: "+07:00",
    text: "N. Central Asia Standard Time (+07:00)",
  },
  {
    value: "Tomsk Standard Time",
    offset: "+07:00",
    text: "Tomsk Standard Time (+07:00)",
  },
  {
    value: "China Standard Time",
    offset: "+08:00",
    text: "China Standard Time (+08:00)",
  },
  {
    value: "North Asia East Standard Time",
    offset: "+08:00",
    text: "North Asia East Standard Time (+08:00)",
  },
  {
    value: "Singapore Standard Time",
    offset: "+08:00",
    text: "Singapore Standard Time (+08:00)",
  },
  {
    value: "W. Australia Standard Time",
    offset: "+08:00",
    text: "W. Australia Standard Time (+08:00)",
  },
  {
    value: "Taipei Standard Time",
    offset: "+08:00",
    text: "Taipei Standard Time (+08:00)",
  },
  {
    value: "Ulaanbaatar Standard Time",
    offset: "+08:00",
    text: "Ulaanbaatar Standard Time (+08:00)",
  },
  {
    value: "Aus Central W. Standard Time",
    offset: "+08:45",
    text: "Aus Central W. Standard Time (+08:45)",
  },
  {
    value: "Transbaikal Standard Time",
    offset: "+09:00",
    text: "Transbaikal Standard Time (+09:00)",
  },
  {
    value: "Tokyo Standard Time",
    offset: "+09:00",
    text: "Tokyo Standard Time (+09:00)",
  },
  {
    value: "North Korea Standard Time",
    offset: "+09:00",
    text: "North Korea Standard Time (+09:00)",
  },
  {
    value: "Korea Standard Time",
    offset: "+09:00",
    text: "Korea Standard Time (+09:00)",
  },
  {
    value: "Yakutsk Standard Time",
    offset: "+09:00",
    text: "Yakutsk Standard Time (+09:00)",
  },
  {
    value: "Cen. Australia Standard Time",
    offset: "+09:30",
    text: "Cen. Australia Standard Time (+09:30)",
  },
  {
    value: "AUS Central Standard Time",
    offset: "+09:30",
    text: "AUS Central Standard Time (+09:30)",
  },
  {
    value: "E. Australia Standard Time",
    offset: "+10:00",
    text: "E. Australia Standard Time (+10:00)",
  },
  {
    value: "AUS Eastern Standard Time",
    offset: "+10:00",
    text: "AUS Eastern Standard Time (+10:00)",
  },
  {
    value: "West Pacific Standard Time",
    offset: "+10:00",
    text: "West Pacific Standard Time (+10:00)",
  },
  {
    value: "Tasmania Standard Time",
    offset: "+10:00",
    text: "Tasmania Standard Time (+10:00)",
  },
  {
    value: "Vladivostok Standard Time",
    offset: "+10:00",
    text: "Vladivostok Standard Time (+10:00)",
  },
  {
    value: "Lord Howe Standard Time",
    offset: "+10:30",
    text: "Lord Howe Standard Time (+10:30)",
  },
  {
    value: "Bougainville Standard Time",
    offset: "+11:00",
    text: "Bougainville Standard Time (+11:00)",
  },
  {
    value: "Russia Time Zone 10",
    offset: "+11:00",
    text: "Russia Time Zone 10 (+11:00)",
  },
  {
    value: "Magadan Standard Time",
    offset: "+11:00",
    text: "Magadan Standard Time (+11:00)",
  },
  {
    value: "Norfolk Standard Time",
    offset: "+11:00",
    text: "Norfolk Standard Time (+11:00)",
  },
  {
    value: "Sakhalin Standard Time",
    offset: "+11:00",
    text: "Sakhalin Standard Time (+11:00)",
  },
  {
    value: "Central Pacific Standard Time",
    offset: "+11:00",
    text: "Central Pacific Standard Time (+11:00)",
  },
  {
    value: "Russia Time Zone 11",
    offset: "+12:00",
    text: "Russia Time Zone 11 (+12:00)",
  },
  {
    value: "New Zealand Standard Time",
    offset: "+12:00",
    text: "New Zealand Standard Time (+12:00)",
  },
  {
    value: "UTC+12",
    offset: "+12:00",
    text: "UTC+12 (+12:00)",
  },
  {
    value: "Fiji Standard Time",
    offset: "+12:00",
    text: "Fiji Standard Time (+12:00)",
  },
  {
    value: "Chatham Islands Standard Time",
    offset: "+12:45",
    text: "Chatham Islands Standard Time (+12:45)",
  },
  {
    value: "UTC+13",
    offset: "+13:00",
    text: "UTC+13 (+13:00)",
  },
  {
    value: "Tonga Standard Time",
    offset: "+13:00",
    text: "Tonga Standard Time (+13:00)",
  },
  {
    value: "Samoa Standard Time",
    offset: "+13:00",
    text: "Samoa Standard Time (+13:00)",
  },
  {
    value: "Kamchatka Standard Time",
    offset: "+13:00",
    text: "Kamchatka Standard Time (+13:00)",
  },
  {
    value: "Line Islands Standard Time",
    offset: "+14:00",
    text: "Line Islands Standard Time (+14:00)",
  },
  {
    value: "Cape Verde Standard Time",
    offset: "-01:00",
    text: "Cape Verde Standard Time (-01:00)",
  },
  {
    value: "Mid-Atlantic Standard Time",
    offset: "-01:00",
    text: "Mid-Atlantic Standard Time (-01:00)",
  },
  {
    value: "UTC-02",
    offset: "-02:00",
    text: "UTC-02 (-02:00)",
  },
  {
    value: "Greenland Standard Time",
    offset: "-02:00",
    text: "Greenland Standard Time (-02:00)",
  },
  {
    value: "Saint Pierre Standard Time",
    offset: "-02:00",
    text: "Saint Pierre Standard Time (-02:00)",
  },
  {
    value: "Newfoundland Standard Time",
    offset: "-02:30",
    text: "Newfoundland Standard Time (-02:30)",
  },
  {
    value: "Tocantins Standard Time",
    offset: "-03:00",
    text: "Tocantins Standard Time (-03:00)",
  },
  {
    value: "E. South America Standard Time",
    offset: "-03:00",
    text: "E. South America Standard Time (-03:00)",
  },
  {
    value: "SA Eastern Standard Time",
    offset: "-03:00",
    text: "SA Eastern Standard Time (-03:00)",
  },
  {
    value: "Argentina Standard Time",
    offset: "-03:00",
    text: "Argentina Standard Time (-03:00)",
  },
  {
    value: "Atlantic Standard Time",
    offset: "-03:00",
    text: "Atlantic Standard Time (-03:00)",
  },
  {
    value: "Bahia Standard Time",
    offset: "-03:00",
    text: "Bahia Standard Time (-03:00)",
  },
  {
    value: "Montevideo Standard Time",
    offset: "-03:00",
    text: "Montevideo Standard Time (-03:00)",
  },
  {
    value: "Magallanes Standard Time",
    offset: "-03:00",
    text: "Magallanes Standard Time (-03:00)",
  },
  {
    value: "Venezuela Standard Time",
    offset: "-04:00",
    text: "Venezuela Standard Time (-04:00)",
  },
  {
    value: "Central Brazilian Standard Time",
    offset: "-04:00",
    text: "Central Brazilian Standard Time (-04:00)",
  },
  {
    value: "SA Western Standard Time",
    offset: "-04:00",
    text: "SA Western Standard Time (-04:00)",
  },
  {
    value: "Pacific SA Standard Time",
    offset: "-04:00",
    text: "Pacific SA Standard Time (-04:00)",
  },
  {
    value: "Eastern Standard Time",
    offset: "-04:00",
    text: "Eastern Standard Time (-04:00)",
  },
  {
    value: "Haiti Standard Time",
    offset: "-04:00",
    text: "Haiti Standard Time (-04:00)",
  },
  {
    value: "Cuba Standard Time",
    offset: "-04:00",
    text: "Cuba Standard Time (-04:00)",
  },
  {
    value: "US Eastern Standard Time",
    offset: "-04:00",
    text: "US Eastern Standard Time (-04:00)",
  },
  {
    value: "Turks And Caicos Standard Time",
    offset: "-04:00",
    text: "Turks And Caicos Standard Time (-04:00)",
  },
  {
    value: "Paraguay Standard Time",
    offset: "-04:00",
    text: "Paraguay Standard Time (-04:00)",
  },
  {
    value: "SA Pacific Standard Time",
    offset: "-05:00",
    text: "SA Pacific Standard Time (-05:00)",
  },
  {
    value: "Eastern Standard Time (Mexico)",
    offset: "-05:00",
    text: "Eastern Standard Time (Mexico) (-05:00)",
  },
  {
    value: "Central Standard Time",
    offset: "-05:00",
    text: "Central Standard Time (-05:00)",
  },
  {
    value: "Easter Island Standard Time",
    offset: "-06:00",
    text: "Easter Island Standard Time (-06:00)",
  },
  {
    value: "Central Standard Time (Mexico)",
    offset: "-06:00",
    text: "Central Standard Time (Mexico) (-06:00)",
  },
  {
    value: "Canada Central Standard Time",
    offset: "-06:00",
    text: "Canada Central Standard Time (-06:00)",
  },
  {
    value: "Mountain Standard Time",
    offset: "-06:00",
    text: "Mountain Standard Time (-06:00)",
  },
  {
    value: "Central America Standard Time",
    offset: "-06:00",
    text: "Central America Standard Time (-06:00)",
  },
  {
    value: "Yukon Standard Time",
    offset: "-07:00",
    text: "Yukon Standard Time (-07:00)",
  },
  {
    value: "Pacific Standard Time",
    offset: "-07:00",
    text: "Pacific Standard Time (-07:00)",
  },
  {
    value: "US Mountain Standard Time",
    offset: "-07:00",
    text: "US Mountain Standard Time (-07:00)",
  },
  {
    value: "Mountain Standard Time (Mexico)",
    offset: "-07:00",
    text: "Mountain Standard Time (Mexico) (-07:00)",
  },
  {
    value: "Pacific Standard Time (Mexico)",
    offset: "-07:00",
    text: "Pacific Standard Time (Mexico) (-07:00)",
  },
  {
    value: "UTC-08",
    offset: "-08:00",
    text: "UTC-08 (-08:00)",
  },
  {
    value: "Alaskan Standard Time",
    offset: "-08:00",
    text: "Alaskan Standard Time (-08:00)",
  },
  {
    value: "UTC-09",
    offset: "-09:00",
    text: "UTC-09 (-09:00)",
  },
  {
    value: "Aleutian Standard Time",
    offset: "-09:00",
    text: "Aleutian Standard Time (-09:00)",
  },
  {
    value: "Marquesas Standard Time",
    offset: "-09:30",
    text: "Marquesas Standard Time (-09:30)",
  },
  {
    value: "Hawaiian Standard Time",
    offset: "-10:00",
    text: "Hawaiian Standard Time (-10:00)",
  },
  {
    value: "UTC-11",
    offset: "-11:00",
    text: "UTC-11 (-11:00)",
  },
  {
    value: "Dateline Standard Time",
    offset: "-12:00",
    text: "Dateline Standard Time (-12:00)",
  },
];

const timezoneService = {
  getTimezoneText() {
    return timezonesOptions.map((zone) => zone.text);
  },
  getAllTimezones() {
    return timezonesOptions;
  },
};

export default timezoneService;
