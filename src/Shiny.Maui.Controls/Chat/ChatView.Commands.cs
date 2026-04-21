using System.Collections.Specialized;
using System.Windows.Input;
using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView
{
    void OnMessagesChanged()
    {
        // Unsubscribe from old collection
        if (observedCollection is not null)
        {
            observedCollection.CollectionChanged -= OnMessagesCollectionChanged;
            observedCollection = null;
        }

        collectionView.ItemsSource = Messages;

        // Subscribe to new collection
        if (Messages is INotifyCollectionChanged ncc)
        {
            ncc.CollectionChanged += OnMessagesCollectionChanged;
            observedCollection = ncc;
        }

        isNearBottom = true;
        unreadCount = 0;
        newMessagesPill.IsVisible = false;
        PerformInitialScroll();
    }

    void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
            return;

        // Only handle appended messages (new messages), not prepended (load more)
        if (e.NewStartingIndex < 0 || Messages is not { Count: > 0 } || e.NewStartingIndex < Messages.Count - 1)
            return;

        var newMessage = Messages[e.NewStartingIndex];

        // Always scroll to bottom for own messages
        if (newMessage.IsFromMe || isNearBottom)
        {
            unreadCount = 0;
            UpdateNewMessagesPill();
            PerformInitialScroll();
        }
        else
        {
            // Incoming message while scrolled up — show "New Messages" pill
            unreadCount++;
            UpdateNewMessagesPill();
        }
    }

    void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        // Consider "near bottom" if within ~50px of the end or last item is visible
        var lastIndex = (Messages?.Count ?? 0) - 1;
        if (lastIndex < 0)
        {
            isNearBottom = true;
            return;
        }

        isNearBottom = e.LastVisibleItemIndex >= lastIndex - 1;

        if (isNearBottom && unreadCount > 0)
        {
            unreadCount = 0;
            UpdateNewMessagesPill();
        }
    }

    void UpdateNewMessagesPill()
    {
        if (unreadCount <= 0)
        {
            newMessagesPill.IsVisible = false;
            return;
        }

        newMessagesPillLabel.Text = unreadCount == 1
            ? "1 New Message"
            : $"{unreadCount} New Messages";
        newMessagesPill.IsVisible = true;
    }

    void OnNewMessagesPillTapped(object? sender, TappedEventArgs e)
    {
        unreadCount = 0;
        newMessagesPill.IsVisible = false;
        ScrollToEnd(animate: true);
    }

    void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (isLoadingMore)
            return;

        if (LoadMoreCommand?.CanExecute(null) != true)
            return;

        isLoadingMore = true;
        Dispatcher.Dispatch(() =>
        {
            try
            {
                LoadMoreCommand?.Execute(null);
            }
            finally
            {
                // Delay resetting the flag so the CollectionView can settle
                Dispatcher.Dispatch(() => isLoadingMore = false);
            }
        });
    }

    void OnSendRequested(string text)
    {
        if (UseHapticFeedback)
            HapticHelper.PerformClick();

        if (SendCommand?.CanExecute(text) == true)
            SendCommand.Execute(text);
    }

    void OnAttachRequested()
    {
        if (UseHapticFeedback)
            HapticHelper.PerformClick();

        if (AttachImageCommand?.CanExecute(null) == true)
            AttachImageCommand.Execute(null);
    }

    internal void OnMessageTapped(ChatMessage message)
    {
        MessageTapped?.Invoke(this, message);

        if (MessageTappedCommand?.CanExecute(message) == true)
            MessageTappedCommand.Execute(message);
    }

    void PerformInitialScroll()
    {
        if (Messages is not { Count: > 0 })
            return;

        Dispatcher.Dispatch(() =>
        {
            if (Messages is not { Count: > 0 })
                return;

            if (ScrollToFirstUnread && FirstUnreadMessageId is not null)
                ScrollToMessage(FirstUnreadMessageId);
            else
                ScrollToEnd();
        });
    }

    public void ScrollToEnd(bool animate = false)
    {
        if (Messages is { Count: > 0 })
            collectionView.ScrollTo(Messages.Count - 1, position: ScrollToPosition.End, animate: animate);
    }

    public void ScrollToMessage(string messageId, bool animate = true)
    {
        if (Messages is null)
            return;

        for (var i = 0; i < Messages.Count; i++)
        {
            if (Messages[i].Id == messageId)
            {
                collectionView.ScrollTo(i, position: ScrollToPosition.Start, animate: animate);
                return;
            }
        }

        ScrollToEnd(animate);
    }

    void RefreshBubbles()
    {
        // Must null in one frame and re-set in the next so CollectionView sees a real change
        collectionView.ItemsSource = null;
        Dispatcher.Dispatch(() => collectionView.ItemsSource = Messages);
    }

    void OnTypingParticipantsChanged()
    {
        // Unsubscribe from old collection
        if (observedTypingCollection is not null)
        {
            observedTypingCollection.CollectionChanged -= OnTypingCollectionChanged;
            observedTypingCollection = null;
        }

        // Subscribe to new collection
        if (TypingParticipants is INotifyCollectionChanged ncc)
        {
            ncc.CollectionChanged += OnTypingCollectionChanged;
            observedTypingCollection = ncc;
        }

        UpdateTypingIndicator();
    }

    void OnTypingCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateTypingIndicator();
    }

    void UpdateTypingIndicator()
    {
        if (!ShowTypingIndicator || TypingParticipants is not { Count: > 0 })
        {
            typingIndicator.IsVisible = false;
            return;
        }

        var participants = TypingParticipants;
        typingIndicator.IsVisible = true;

        typingIndicator.Text = participants.Count switch
        {
            1 => $"{participants[0].DisplayName} is typing\u2026",
            2 => $"{participants[0].DisplayName}, {participants[1].DisplayName} are typing\u2026",
            3 => $"{participants[0].DisplayName}, {participants[1].DisplayName}, {participants[2].DisplayName} are typing\u2026",
            _ => "Multiple users are typing\u2026"
        };
    }
}
