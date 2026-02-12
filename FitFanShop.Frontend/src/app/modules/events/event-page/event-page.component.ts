import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { EventApiService } from '../../../api-services/events/event-api.service';
import { EventDto } from '../../../api-services/events/event-api.model';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-event-page',
  standalone: false,
  templateUrl: './event-page.component.html',
  styleUrl: './event-page.component.scss'
})
export class EventPageComponent implements OnInit {
  private eventService = inject(EventApiService);
  private router = inject(Router);

  events = signal<EventDto[]>([]);
  minTicketPrices = signal<Record<number, number>>({});
  isLoading = signal<boolean>(true);

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading.set(true);
    this.eventService.getUpcomingEvents().subscribe({
      next: (events) => {
        this.events.set(events);
        this.loadMinTicketPrices(events);
        this.isLoading.set(false);
      },
      error: () => {
        this.events.set([]);
        this.minTicketPrices.set({});
        this.isLoading.set(false);
      }
    });
  }

  loadMinTicketPrices(events: EventDto[]): void {
    if (events.length === 0) {
      this.minTicketPrices.set({});
      return;
    }

    const requests = events.map(event =>
      this.eventService.getById(event.id).pipe(
        catchError(() => of(null))
      )
    );

    forkJoin(requests).subscribe(results => {
      const prices: Record<number, number> = {};

      results.forEach((details, index) => {
        const eventId = events[index].id;
        if (!details || !details.ticketTypes || details.ticketTypes.length === 0) {
          return;
        }

        const availablePrices = details.ticketTypes
          .filter(ticket => ticket.price > 0)
          .map(ticket => ticket.price);

        if (availablePrices.length > 0) {
          prices[eventId] = Math.min(...availablePrices);
        }
      });

      this.minTicketPrices.set(prices);
    });
  }

  minPriceFor(eventId: number): number | null {
    const price = this.minTicketPrices()[eventId];
    return typeof price === 'number' ? price : null;
  }

  openEvent(eventId: number): void {
    this.router.navigate(['/event', eventId]);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
