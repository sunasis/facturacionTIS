﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{65510B7D-524E-424E-9BC5-6B1200389A6C}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>FacturacionElectronica.Homologacion</RootNamespace>
    <AssemblyName>FacturacionElectronica.Homologacion</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <TargetFrameworkProfile />
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>bin\Release\FacturacionElectronica.Homologacion.XML</DocumentationFile>
  </PropertyGroup>
  <PropertyGroup>
    <SignAssembly>true</SignAssembly>
  </PropertyGroup>
  <PropertyGroup>
    <AssemblyOriginatorKeyFile>FacturacionTIS.pfx</AssemblyOriginatorKeyFile>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Ionic.Zip.Reduced">
      <HintPath>C:\Users\Administrador\Dropbox\Libs\Ionic.Zip.Reduced.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Runtime.Serialization" />
    <Reference Include="System.ServiceModel" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Xml" />
    <Reference Include="UblLarsen.Ubl2, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b1e81a24f6c09f95, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\FacturacionElectronica.GeneradorXml\Lib\UblLarsen.Ubl2.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Properties\AssemblyInfo.cs" />
    <Compile Include="Resources.Designer.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>Resources.resx</DependentUpon>
    </Compile>
    <Compile Include="Res\Enums.cs" />
    <Compile Include="Res\ProcessXML.cs" />
    <Compile Include="Res\ProcessZip.cs" />
    <Compile Include="Res\Response.cs" />
    <Compile Include="Security\ServiceHelper.cs" />
    <Compile Include="Service References\ClientServiceConsult\Reference.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>Reference.svcmap</DependentUpon>
    </Compile>
    <Compile Include="Service References\ClientService\Reference.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>Reference.svcmap</DependentUpon>
    </Compile>
    <Compile Include="Service References\GuiaRemisionService\Reference.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>Reference.svcmap</DependentUpon>
    </Compile>
    <Compile Include="Settings.Designer.cs">
      <AutoGen>True</AutoGen>
      <DesignTimeSharedInput>True</DesignTimeSharedInput>
      <DependentUpon>Settings.settings</DependentUpon>
    </Compile>
    <Compile Include="SunatCe.cs" />
    <Compile Include="SunatGuiaRemision.cs" />
    <Compile Include="SunatManager.cs" />
    <Compile Include="SunatCeR.cs" />
  </ItemGroup>
  <ItemGroup>
    <WCFMetadata Include="Service References\" />
  </ItemGroup>
  <ItemGroup>
    <None Include="app.config" />
    <None Include="FacturacionTIS.pfx" />
    <None Include="Service References\ClientServiceConsult\billConsultService.wsdl" />
    <None Include="Service References\ClientServiceConsult\billConsultService.xsd">
      <SubType>Designer</SubType>
    </None>
    <None Include="Service References\ClientServiceConsult\billConsultService1.wsdl" />
    <None Include="Service References\ClientServiceConsult\FacturacionElectronica.Homologacion.ClientServiceConsult.statusResponse.datasource">
      <DependentUpon>Reference.svcmap</DependentUpon>
    </None>
    <None Include="Service References\ClientService\billService.wsdl" />
    <None Include="Service References\ClientService\billService.xsd">
      <SubType>Designer</SubType>
    </None>
    <None Include="Service References\ClientService\billService1.wsdl" />
    <None Include="Service References\ClientService\FacturacionElectronica.Homologacion.ClientService.statusResponse.datasource">
      <DependentUpon>Reference.svcmap</DependentUpon>
    </None>
    <None Include="Service References\GuiaRemisionService\billService.wsdl" />
    <None Include="Service References\GuiaRemisionService\billService.xsd">
      <SubType>Designer</SubType>
    </None>
    <None Include="Service References\GuiaRemisionService\billService1.wsdl" />
    <None Include="Service References\GuiaRemisionService\FacturacionElectronica.Homologacion.GuiaRemisionService.statusResponse.datasource">
      <DependentUpon>Reference.svcmap</DependentUpon>
    </None>
    <None Include="Settings.settings">
      <Generator>SettingsSingleFileGenerator</Generator>
      <LastGenOutput>Settings.Designer.cs</LastGenOutput>
    </None>
  </ItemGroup>
  <ItemGroup>
    <WCFMetadataStorage Include="Service References\ClientServiceConsult\" />
    <WCFMetadataStorage Include="Service References\ClientService\" />
    <WCFMetadataStorage Include="Service References\GuiaRemisionService\" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientService\configuration91.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientService\configuration.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientService\Reference.svcmap">
      <Generator>WCF Proxy Generator</Generator>
      <LastGenOutput>Reference.cs</LastGenOutput>
    </None>
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientServiceConsult\configuration91.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientServiceConsult\configuration.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\ClientServiceConsult\Reference.svcmap">
      <Generator>WCF Proxy Generator</Generator>
      <LastGenOutput>Reference.cs</LastGenOutput>
    </None>
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Resources.resx">
      <Generator>ResXFileCodeGenerator</Generator>
      <LastGenOutput>Resources.Designer.cs</LastGenOutput>
      <SubType>Designer</SubType>
    </EmbeddedResource>
  </ItemGroup>
  <ItemGroup>
    <None Include="Resources\ListadeErrores.xml" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\GuiaRemisionService\configuration91.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\GuiaRemisionService\configuration.svcinfo" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Service References\GuiaRemisionService\Reference.svcmap">
      <Generator>WCF Proxy Generator</Generator>
      <LastGenOutput>Reference.cs</LastGenOutput>
    </None>
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