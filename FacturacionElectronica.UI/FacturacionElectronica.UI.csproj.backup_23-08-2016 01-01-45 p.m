﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{2A8CED1C-28F0-44F3-9F17-C26F7E019C01}</ProjectGuid>
    <OutputType>WinExe</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>FacturacionElectronica.UI</RootNamespace>
    <AssemblyName>FacturacionElectronica.UI</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <TargetFrameworkProfile />
    <PathToSmartAssembly>C:\Program Files\Red Gate\SmartAssembly 6</PathToSmartAssembly>
    <IsWebBootstrapper>false</IsWebBootstrapper>
    <PublishUrl>C:\Users\Administrador\Documents\Docs\</PublishUrl>
    <Install>true</Install>
    <InstallFrom>Disk</InstallFrom>
    <UpdateEnabled>false</UpdateEnabled>
    <UpdateMode>Foreground</UpdateMode>
    <UpdateInterval>7</UpdateInterval>
    <UpdateIntervalUnits>Days</UpdateIntervalUnits>
    <UpdatePeriodically>false</UpdatePeriodically>
    <UpdateRequired>false</UpdateRequired>
    <MapFileExtensions>true</MapFileExtensions>
    <PublisherName>Giancarlos</PublisherName>
    <SuiteName>Facturacion</SuiteName>
    <ApplicationRevision>1</ApplicationRevision>
    <ApplicationVersion>1.0.0.%2a</ApplicationVersion>
    <UseApplicationTrust>false</UseApplicationTrust>
    <PublishWizardCompleted>true</PublishWizardCompleted>
    <BootstrapperEnabled>true</BootstrapperEnabled>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>
    </DocumentationFile>
  </PropertyGroup>
  <PropertyGroup>
    <SignManifests>true</SignManifests>
  </PropertyGroup>
  <PropertyGroup>
    <ManifestCertificateThumbprint>DC20AD2CC5106D3D17D48B80CFDEB0BEA514D39B</ManifestCertificateThumbprint>
  </PropertyGroup>
  <PropertyGroup>
    <TargetZone>LocalIntranet</TargetZone>
  </PropertyGroup>
  <PropertyGroup>
    <GenerateManifests>true</GenerateManifests>
  </PropertyGroup>
  <PropertyGroup>
    <ApplicationManifest>Properties\app.manifest</ApplicationManifest>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Runtime.Serialization.Formatters.Soap" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Deployment" />
    <Reference Include="System.Drawing" />
    <Reference Include="System.Windows.Forms" />
    <Reference Include="System.Xml" />
    <Reference Include="UblLarsen.Ubl2, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b1e81a24f6c09f95, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\FacturacionElectronica.GeneradorXml\Lib\UblLarsen.Ubl2.dll</HintPath>
    </Reference>
    <Reference Include="UIAutomationProvider" />
    <Reference Include="UIAutomationTypes" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include="frmTest.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="frmTest.Designer.cs">
      <DependentUpon>frmTest.cs</DependentUpon>
    </Compile>
    <Compile Include="MainForm.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="MainForm.designer.cs">
      <DependentUpon>MainForm.cs</DependentUpon>
    </Compile>
    <Compile Include="Program.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
    <EmbeddedResource Include="frmTest.resx">
      <DependentUpon>frmTest.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="MainForm.resx">
      <DependentUpon>MainForm.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="Properties\Resources.resx">
      <Generator>ResXFileCodeGenerator</Generator>
      <LastGenOutput>Resources.Designer.cs</LastGenOutput>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <Compile Include="Properties\Resources.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Resources.resx</DependentUpon>
      <DesignTime>True</DesignTime>
    </Compile>
    <None Include="Properties\app.manifest" />
    <None Include="Properties\Settings.settings">
      <Generator>SettingsSingleFileGenerator</Generator>
      <LastGenOutput>Settings.Designer.cs</LastGenOutput>
    </None>
    <Compile Include="Properties\Settings.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Settings.settings</DependentUpon>
      <DesignTimeSharedInput>True</DesignTimeSharedInput>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <None Include="App.config">
      <SubType>Designer</SubType>
    </None>
  </ItemGroup>
  <ItemGroup>
    <WCFMetadata Include="Service References\" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\FacturacionElectronica.GeneradorXml\FacturacionElectronica.GeneradorXml.csproj">
      <Project>{3935f0d2-9563-46c4-86d9-d459e8a90737}</Project>
      <Name>FacturacionElectronica.GeneradorXml</Name>
    </ProjectReference>
    <ProjectReference Include="..\FacturacionElectronica.Homologacion\FacturacionElectronica.Homologacion.csproj">
      <Project>{65510b7d-524e-424e-9bc5-6b1200389a6c}</Project>
      <Name>FacturacionElectronica.Homologacion</Name>
    </ProjectReference>
  </ItemGroup>
  <ItemGroup>
    <BootstrapperPackage Include=".NETFramework,Version=v4.0">
      <Visible>False</Visible>
      <ProductName>Microsoft .NET Framework 4 %28x86 y x64%29</ProductName>
      <Install>true</Install>
    </BootstrapperPackage>
    <BootstrapperPackage Include="Microsoft.Net.Framework.3.5.SP1">
      <Visible>False</Visible>
      <ProductName>.NET Framework 3.5 SP1</ProductName>
      <Install>false</Install>
    </BootstrapperPackage>
    <BootstrapperPackage Include="Microsoft.Windows.Installer.4.5">
      <Visible>False</Visible>
      <ProductName>Windows Installer 4.5</ProductName>
      <Install>true</Install>
    </BootstrapperPackage>
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name="BeforeBuild">
  </Target>
  <Target Name="AfterBuild">
  </Target>
  -->
</Project>