export interface EventDto {
  id: number;
  name: string;
  eventDate: string;
  location: string;
  hasPassed: boolean;
  ticketsAvailable: boolean;
  totalTicketTypes: number;
}

export interface TicketTypeDto {
  id: number;
  name: string;
  price: number;
  totalAvailable: number;
  description?: string;
  isAvailable: boolean;
}

export interface EventDetailsDto {
  id: number;
  name: string;
  description?: string;
  eventDate: string;
  location: string;
  hasPassed: boolean;
  ticketsAvailable: boolean;
  ticketTypes: TicketTypeDto[];
}
