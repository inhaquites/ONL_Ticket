using AutoMapper;
using Lenovo.NAT.Infrastructure.Entities;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Lenovo.NAT.ViewModel.Logistic.OnlTicket;
using Lenovo.NAT.ViewModel.Logistic.Picking;
using Lenovo.NAT.ViewModel.User;

namespace Lenovo.NAT.Infrastructure.AutoMapper;

public class ConfigurationMapping : Profile
{
    public ConfigurationMapping()
    {
        CreateMap<ModuleViewModel, Module>().ReverseMap();
        
        CreateMap<User, UserViewModel>().ReverseMap();

        //LOGISTIC - PICKING
        CreateMap<PickingViewModel, Picking>().ReverseMap();
        CreateMap<PickingAreaViewModel, PickingArea>().ReverseMap();
        CreateMap<PickingStatusViewModel, PickingStatus>().ReverseMap();
        CreateMap<PickingReasonViewModel, PickingReason>().ReverseMap();
        CreateMap<PickingCarrierViewModel, PickingCarrier>().ReverseMap();
        CreateMap<PickingBrandViewModel, PickingBrand>().ReverseMap();
        CreateMap<PickingItemViewModel, PickingItem>().ReverseMap();
        CreateMap<PickingInvoiceViewModel, PickingInvoice>().ReverseMap();
        CreateMap<PickingProcessTypeViewModel, PickingProcessType>().ReverseMap();
       
        CreateMap<LogisticInvoice, LogisticInvoiceViewModel>().ReverseMap();


        //LOGISTIC - ONL TICKET
        CreateMap<OnlTicketOrderTypeViewModel, OrderType>().ReverseMap();
        CreateMap<OnlTicketOrderStatusViewModel, OrderStatus>().ReverseMap();
        CreateMap<OnlTicketNFTypeViewModel, NFType>().ReverseMap();
        CreateMap<OnlTicketCustomerSegmentViewModel, CustomerSegment>().ReverseMap();
        CreateMap<CountryViewModel, Country>().ReverseMap();
        
        // Order Entities - MIGRAÇÃO COMPLETA
        CreateMap<OnlTicketViewModel, OrderNotLoaded>().ReverseMap();
        CreateMap<OnlTicketAttachmentViewModel, OrderAttachment>().ReverseMap();
        CreateMap<SoldToViewModel, OrderSoldTO>().ReverseMap();
        CreateMap<ShipToViewModel, OrderShipTo>().ReverseMap();
        CreateMap<OrderItemViewModel, OrderProduct>().ReverseMap();
    }
}
