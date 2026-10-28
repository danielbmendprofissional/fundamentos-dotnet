// Scenario A: Basic Control with Samsung
using DesignPatterns.Bridge.Good.EixoAbstracao;
using DesignPatterns.Bridge.Good.EixoAbstracao.Base;
using DesignPatterns.Bridge.Good.EixoImplementacao;

var basicSamsung = new BasicRemote(new SamsungTv());
basicSamsung.TogglePower();

// Scenario B: Smart Control with LG
var smartLg = new SmartRemote(new LgTv());
smartLg.OpenNetflix();

// Scenario C: Smart Control with Samsung (The Bridge Magic!)
var smartSamsung = new SmartRemote(new SamsungTv());
smartSamsung.OpenNetflix();