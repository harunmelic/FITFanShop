import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { EventApiService } from '../../../api-services/events/event-api.service';
import { EventDetailsDto, TicketTypeDto } from '../../../api-services/events/event-api.model';
import { CartService } from '../../../core/services/cart/cart.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { MatDialog } from '@angular/material/dialog';
import { LoginDialogComponent } from '../../shared/components/login-dialog/login-dialog.component';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-event-detail',
  standalone: false,
  templateUrl: './event-detail.component.html',
  styleUrl: './event-detail.component.scss'
})
export class EventDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private eventService = inject(EventApiService);
  private cartService = inject(CartService);
  private auth = inject(AuthFacadeService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);

  event = signal<EventDetailsDto | null>(null);
  isLoading = signal<boolean>(true);
  isAdding = signal<boolean>(false);

  quantityForm: FormGroup = this.fb.group({});

  eventDateFormatted = computed(() => {
    const event = this.event();
    if (!event) return '';

    return new Date(event.eventDate).toLocaleString('en-GB', {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  });

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const eventId = +params['id'];
      if (!eventId) {
        this.router.navigate(['/event']);
        return;
      }
      this.loadEvent(eventId);
    });
  }

  loadEvent(eventId: number): void {
    this.isLoading.set(true);
    this.eventService.getById(eventId).subscribe({
      next: (event) => {
        this.event.set(event);
        this.initQuantityForm(event.ticketTypes);
        this.isLoading.set(false);
      },
      error: () => {
        this.toaster.error('Failed to load event details');
        this.router.navigate(['/event']);
        this.isLoading.set(false);
      }
    });
  }

  initQuantityForm(ticketTypes: TicketTypeDto[]): void {
    const controls: Record<string, number> = {};
    ticketTypes.forEach(ticket => {
      controls[`ticket_${ticket.id}`] = 0;
    });
    this.quantityForm = this.fb.group(controls);
  }

  decrement(ticketTypeId: number): void {
    const key = `ticket_${ticketTypeId}`;
    const current = this.quantityForm.get(key)?.value || 0;
    if (current > 0) {
      this.quantityForm.get(key)?.setValue(current - 1);
    }
  }

  increment(ticket: TicketTypeDto): void {
    const key = `ticket_${ticket.id}`;
    const current = this.quantityForm.get(key)?.value || 0;
    if (current < ticket.totalAvailable) {
      this.quantityForm.get(key)?.setValue(current + 1);
    }
  }

  selectedCount(ticketTypeId: number): number {
    return this.quantityForm.get(`ticket_${ticketTypeId}`)?.value || 0;
  }

  totalSelectedTickets(): number {
    const event = this.event();
    if (!event) return 0;

    return event.ticketTypes.reduce((sum, t) => sum + this.selectedCount(t.id), 0);
  }

  totalPrice(): number {
    const event = this.event();
    if (!event) return 0;

    return event.ticketTypes.reduce((sum, t) => sum + this.selectedCount(t.id) * t.price, 0);
  }

  addSelectedToCart(): void {
    const event = this.event();
    if (!event || this.totalSelectedTickets() === 0) {
      this.toaster.warning('Please select at least one ticket');
      return;
    }

    if (!this.auth.isAuthenticated()) {
      this.dialog.open(LoginDialogComponent, {
        width: '400px',
        disableClose: false
      });
      return;
    }

    this.isAdding.set(true);

    const selected = event.ticketTypes
      .map(ticket => ({ ticketTypeId: ticket.id, quantity: this.selectedCount(ticket.id) }))
      .filter(x => x.quantity > 0);

    selected.forEach(ticket => {
      this.cartService.addTicket(ticket.ticketTypeId, ticket.quantity);
    });

    this.isAdding.set(false);
    this.toaster.success('Selected tickets added to cart');

    // Keep user on the current event page after adding tickets
    this.quantityForm.reset(
      Object.keys(this.quantityForm.controls).reduce((acc, key) => {
        acc[key] = 0;
        return acc;
      }, {} as Record<string, number>)
    );
  }

  goBack(): void {
    this.router.navigate(['/event']);
  }
}
